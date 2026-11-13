# MCP server

An embedded [Model Context Protocol](https://modelcontextprotocol.io) server lets an AI client (for example Claude Code) drive Word and the add-in's forms.

- Transport: MCP "Streamable HTTP", JSON responses only, stateless. Protocol versions `2025-06-18` and `2025-03-26`.
- Listens on `http://127.0.0.1:<port>/mcp/` only (loopback). Default port `39181`, next free port if taken.
- Off by default.

## Enable

1. In Word, open the **Spaghetti** tab, **Utilities** group, and press the **MCP** toggle button.
2. The state is remembered (`HKCU\Software\Macaroni\Mcp`, `Enabled`). If it is on, the server starts with Word.
3. Press the button again to stop it.

On start the add-in writes `%LOCALAPPDATA%\Spaghetti\mcp.json` and deletes it on stop:

```json
{"url":"http://127.0.0.1:39181/mcp/","token":"<random>","pid":1234}
```

The token is 32 random bytes (base64url) and **changes on every Word start**, so re-register after restarting Word.

## Register with Claude Code

Take `url` and `token` from `mcp.json`:

```
claude mcp add --transport http spaghetti-word http://127.0.0.1:39181/mcp/ --header "Authorization: Bearer <token>"
```

If the port in `mcp.json` is not 39181, use that URL. To update the token: `claude mcp remove spaghetti-word`, then add again.

## Security

- Every request needs `Authorization: Bearer <token>` (constant-time compare), otherwise 401.
- A request with an `Origin` header that is not `null`/localhost/127.0.0.1 is rejected (403): DNS-rebinding guard.
- `GET /mcp/` returns 405.
- No script or code evaluation. Only the fixed tools and actions below exist.
- `doc_open` / `doc_save` paths must be absolute local paths with an allowed extension (`.docx .doc .docm .rtf .txt .srt .vtt`).

## Tools

| Group | Tools |
| --- | --- |
| Word | `word_status`, `doc_list`, `doc_open`, `doc_new`, `doc_activate`, `doc_close`, `doc_save`, `doc_get_text`, `doc_insert_text`, `doc_select`, `doc_find` |
| Forms | `form_list`, `form_open`, `form_close`, `form_controls`, `form_get`, `form_set`, `form_click` |
| Actions | `action_list`, `action_run`, `undo` |

Forms: `spotlight`, `wordprep`, `fontsreport`, `fontchanger`, `subtitles`, `shortcuts`, `colorpicker`, `about`.

Actions (`action_run`, see `action_list` for arguments): `text.prepare`, `tables.prepare`, `spotlight.run`, `spotlight.clear`, `text.global_hide`, `text.global_unhide`, `fonts.replace`, `fonts.set_latin`, `fonts.set_asian`, `fonts.set_rtl`, `fonts.set_other`, `fonts.document_fonts`, `subtitles.convert`, `images.extract`.

## Behaviour notes

- Every tool runs on Word's UI thread (captured in `ThisAddIn_Startup`); HTTP is handled on background threads. A call times out after 120 s with a JSON-RPC error.
- Exceptions become `isError: true` results; Word is never taken down by a bad call.
- Message boxes shown by the add-in during a call are not displayed. They are returned as `uiMessages` in the tool result. Questions (Yes/No) are answered "No".
- Tool results are JSON text in `content[0].text`.

## Code layout

- `McpCore.cs`, `McpServer.cs`: protocol, auth, threading. No Office interop, unit tested with a fake registry (`Tests/McpServerTests.cs`).
- `FormTools.cs`: form registry and control inspector (WinForms only).
- `WordTools.cs`, `ActionTools.cs`: tools over Word and the existing modules.
- `McpHost.cs`: start/stop, `mcp.json`, registry flag.
- Integration test: `Tests.Integration/McpTests.cs` (real hidden Word, real HTTP).
