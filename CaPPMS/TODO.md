# CaPPMS — Running Plan

Working plan for this project. Items move down the list as they complete.

## In Progress
_(none yet)_

## Next
- [ ] Baseline: `dotnet restore`, `dotnet build`, `dotnet test` from CaPPMS/ — confirm clean state
- [ ] Run locally (`dotnet run` in site/) — confirm it boots and the main pages work

## Backlog
_(add items here)_

## Done
- [x] (2026-10-21) CaPPMS teardown Unit A — removed the public idea-submission surface from `site/Shared/ProjectTable.razor`: deleted the editable `<Idea>` popOver form (`@if(selectedIdea != null)` block), the "Create New Project" button in filter-container, the now-unused `selectedIdea` field and `CreateNewProject()` method, dropped `@implements IIDE`, and converted `CloseIdea(Idea idea)` to a no-op stub (nothing invokes it from the list page anymore). Existing projects remain display-only read-only rows (title/description/status); service layer (`AddAsync`) untouched. Build verified: `dotnet build -c Release --no-restore` → 0 errors, only pre-existing warnings in other files.. Also fixed a dead project-list filter input (@bind-Value) found during review — the retained search box now actually filters rows; same latent bug exists on homepage Search projects (PreviousProjects.razor line 22), tracked as follow-up.

---

### Subagent findings — Qwen (qwen/qwen3.8-27b): top‑10 UX recommendations

1. **Real submission feedback.** The idea form shows "Uploading… Thank you for the project idea!" whether save succeeded or failed; on failure nothing is shown and users can't tell if data was kept. Add a distinct success state (reference ID, what happens next) plus an error state that preserves their input.
2. **Group the long submission form.** Flat single-column label/input table (submitter → project → sponsor → attachments → links). Use fieldset-style grouping with visual hierarchy and sticky Submit/Reset bar; explain via helper text how "Self Sponsored" relates to conditionally appearing "Sponsor Information…".
3. **Inline, per-field validation.** Replace top-of-form ValidationSummary: required indicators (Email is not marked Required), Title char limits 5–256 without jokey messages, hint that https:// auto-prefixes Website/GitHub fields; guidance for Email/Phone.
4. **Manageable attachments with stated limits.** Show selected files w/ sizes and per-file remove; state "max 10 / 20 MB each" up front instead of silently dropping oversized or over-limit selections at picker time — warn rather than drop.
5. **Make the project list a review queue.** Status filter defaulting to New (rows sorted alphabetically by status bury unreviewed items last); show submitter contact, submission date, attachment count per row; visible sort-direction indicators; move filters above table.
6. **Remove or fix dead controls.** +/− toggles do nothing and pagination is a no-op — either expand rows into details/comments/actions with working pager/sort headers, or delete inert buttons so only meaningful actions render per row.
7. **Close/Cancel the project panel; guard unsaved work.** Opened detail panel has no close button (no control invokes CloseIdea); navigating away mid-edit silently loses data — add Cancel/Close plus an unsaved-changes warning before navigation/submit.
8. **Make approve and deny explicit, always reachable, audited.** Approve appears only when Status is New *and* entry isn't dirty → vanishes on first edit with no explanation; there's also no Deny action anywhere (only New→Approved→Complete transitions exist). Show both actions consistently for eligible items, confirm destructive ones, record who/when and reflect outcome back into list rows.
9. **Report results of every async operation.** Update/Delete/Export swallow errors — failures produce zero user-visible feedback while buttons stay enabled mid-flight (GitHubService fire-and-forget Octokit calls). Show toasts for success/error; disable actions during flight so users can't double-submit.
10. **Tie auth and status into the workflow.** Gate faculty-only review actions behind a visible "log in to review" prompt — list/actions reachable anonymously, service layer has no authorization checks (Delete fails later at file manager with discarded error → silent failure). Give submitters an email confirmation containing project ID plus timeline New→Approved/Denied→Complete so they can track progress.

### Bugs / issues identified

**Form handling**
- **NullReferenceException on clear:** `Idea.razor` calls `.StartsWith(...)` directly on bound fields (`Url`, `Github`, `VideoLink`) at submit; clearing a field makes the value null and throws before validation.
- **Reset button is inert:** `<button type="reset">` inside Blazor EditForm does not reset C# model — values/staged attachments persist, so resubmit sends stale data.
- **Inconsistent file limits:** validators allow 50 files / 10 MB while UI and appsettings enforce 10/20; a 15 MB file passes selection then fails validation with "Max file size (10) exceeded".
- **Silent attachment rejection:** oversized or over-count selections dropped at picker time without warning.
- **No feedback on failed submit:** `AddAsync` returning false produces no message and form state resets inconsistently; success vs failure indistinguishable to user.
- **UI mutation off UI thread:** fields reassigned inside Task.Run in HandleValidSubmit — circuit thread-safety hazard.
- **Email not required** despite being contact-critical for a faculty-reviewed idea.

**Auth / security**
- Review actions (Approve/Update/Delete/Export) render without `AuthorizeView`; service layer has no auth checks, so anonymous visitors can approve/edit projects. Delete fails later at file manager with discarded error → silent failure.
- Login links in MicrosoftIdentity are relative to current URL and 404 on nested routes — works only because all app routes happen to be top-level.
- `/download` endpoint unauthenticated: anyone guessing a stored path retrieves attachments/exported PDFs containing submitter PII.
- Any authenticated user may delete any comment; no ownership check in CommentEntry.

**Listing page**
- `+`/`−` row toggles flip an unused HashSet and render nothing (dead scaffolding).
- Broken pagination: items pre-sliced to Page 1, UI never advances it, MudTablePager disconnected → unreachable pages.
- Project panel has no close control; CloseIdea reachable only via uninvoked CloseForm — cannot be closed by user.
- "Create New Project" from list renders with ShowFileAttachments=true which hides InputFile: new projects get zero attachments. Dedicated Submit page (SubmitIdea.razor) referenced nowhere / unreachable in HEAD; review panel buttons don't render either.

**Action results**
- DeleteRecord ignores error string.
- ApproveIdea shows "Successfully Created Repository!" even when UpdateAsync fails — misleading success message, plus repos created public with project descriptions via fire-and-forget Octokit calls.
- Race in TryUpdate: KeyNotFoundException if removed between check and update; AddAsync saves attachments before TryAdd → orphaned files on failure.

**PDF export**
- `Convert.ToInt32(GetConfigurationSetting("ASPNETCORE_HTTPS_PORT"))` throws FormatException when Host is localhost with missing port setting.
- Export links hardcoded https:// (broken for local HTTP); footer hardcodes "SWEN670 Summer 2021" and typo "All Right Reserved"; every non-"New" status labeled Approved even if denied/complete.

**Other**
- Static event memory leak: CommentsComponent/PreviousProjects subscribe to ProjectIdeasChanged in OnInitialized but never unsubscribe — handlers accumulate per page view against singleton.
- Local dev points at production Host (umgc-cappms.azurewebsites.net) → locally generated PDFs contain prod links.
- Comment timestamps use DateTime.Now local time, unreliable ordering across timezones.

### What's left
Review both lists and rack-and-stack them into the plan as you see fit; no implementation was requested yet.<
---

## My own top‑10 UX recommendations (my independent review of CaPPMS code)

Grounded in files I actually read: `ProjectList.razor`, `Shared/ProjectTable.razor` (`site/Shared`), `Shared/Idea.razor` (submission form), the download flow, `Shared/LoginDisplay.razor`, `App.razor`, and `Startup.cs`.

1. **Make the list a pending-first queue.** `ProjectList` orders rows by Status alphabetically (`OrderBy(i => i.Status)` → "Approved" < "Complete" < "New"), so unreviewed ideas land last; add a status filter defaulting to New, or rank-based sorting that surfaces items needing faculty action first.
2. **Fix pagination — it's dead UI today.** `ProjectTable` has `<MudTablePager />`, but the page variable is only ever reset (never advanced by any button); wire OnPageChange/next buttons and show total range ("Showing 1–10 of N") so faculty know how many items exist beyond screen one.
3. **Move filters above the table** — `filter-container` renders after `<MudTable>` closes, which is non-standard placement; also extend filter/faculty triage to include submitter contact if available in the model (currently Title/Description/Status only).
4. **Wire or remove dead toggles.** The row +/− buttons flip a HashSet but `RowTemplate` never renders expanded content — either expand rows inline for comments/actions, or delete inert buttons so users aren't trained to click nothing.
5. **Add close/cancel on the opened Idea popOver and guard unsaved work** — `CloseIdea()` exists in ProjectTable but no control invokes it from the list page; navigating away mid-edit silently loses data (no Cancel/Close, no dirty-warning).
6. **Uniform attachment handling across flows.** "Create New Project" renders `<Idea ShowFileAttachments="true">` which hides `InputFile`, so new projects created via the list get zero attachments while editing presumably allows them — make behavior consistent and show selected files with sizes + per-file remove, stating limits up front.
7. **Per-field validation instead of top-of-form only.** ValidationSummary sits above a flat table; add required indicators (Email/Phone unmarked though contact-critical for faculty review), Title char limit 5–256 without jokey messages, and the https:// auto-prefix hint on Website/GitHub.
8. **Distinct success/error states that preserve input.** The "Uploading… Thank you!" banner renders regardless of outcome; failure shows nothing and data may be lost — show an error state keeping entered values plus a reference ID/next steps on success.
9. **Explicit Approve AND Deny, always reachable for eligible items** (approve vanishes when the entry is dirty with no explanation; codebase has no deny action at all). Show both consistently, confirm destructive ones, record who/when and reflect outcomes back into list rows so faculty see status without reopening each project.
10. **Visible auth gate on review actions + submitter tracking.** `ProjectList`/`ProjectTable` render with `<CascadingAuthenticationState>` but no authorization guard even though Startup configures Microsoft Identity Web — today the list/actions are reachable anonymously per code I read; add an explicit "log in to review" prompt and email submitters a confirmation containing project ID plus status timeline New→Approved/Denied→Complete.

## Bugs / issues found in my own review

**Submission form (Idea.razor)**
- **NullReferenceException on cleared fields:** `HandleValidSubmit` calls `.StartsWith(...)` directly on bound Url/Github/VideoLink without null checks — clearing a field throws before validation.
- **Reset button is inert vs the model.** `<button type="reset">` inside EditForm has no `@onclick`, so native reset never touches the C# `idea` binding; values and staged attachments persist, resubmit sends stale data. Verified by reading file: zero handler attached to that button.
- **Silent attachment rejection at picker time:** `LoadFiles` caps with `.Take(MaxNumberOfFiles)` then skips oversized files via bare `continue` — user never told why selections vanished (limits stated nowhere up front).
- **No feedback when AddAsync returns false.** Success clears state; the failure path is silent and form resets inconsistently, so success vs. failure indistinguishable to submitter.
- **Off-circuit mutation in Task.Run:** fields (`idea`, `loadedFiles`) reassigned inside an async task running on a background thread — circuit thread-safety hazard (verifiable from file lines 244–256).
- **Sponsor checkbox mutates sponsor state even when form invalid** and clears it via ResetSponsor() without user intent context.
- **Dead scaffolding:** `FormButtons` enum has a single value gating an always‑true condition; unused state (`IsUploading`, `Error`, `DownloadFileName`) — all verifiable from the file itself.

**Listing page**
- **Pagination dead.** Page never advances by UI (only reset on column sort); MudTablePager disconnected → pages beyond 10 unreachable.
- **+/− toggles render nothing when expanded**; RowTemplate shows only title/description/status buttons, no details/comments/actions — confirmed from file markup.
- **Status alphabetical sort buries New last** (`OrderBy(i => i.Status)` in ProjectList.razor line 8).
- **Filter box below table**, searches Title/Description/Status only; no submitter contact column/filter for triage.

**Auth / security**
- Review actions reachable anonymously today: `ProjectList`/`ProjectTable` render with `<CascadingAuthenticationState>` but no authorization guard, while Startup configures Microsoft Identity Web and other pages (`Users`, UMGCHeader) do use `<AuthorizeView>` + consent handler — inconsistent coverage.
- Login links in LoginDisplay are relative to current URL (MicrosoftIdentity/Account/SignIn|SignOut), which 404s on nested routes; works only because all app routes happen to be top-level.

**Other**
- **"Create New Project" hides file input:** ShowFileAttachments=true → InputFile hidden, so list-flow projects get zero attachments.

---

## Plan: CaPPMS UX/Bug/Security Review

### Phase 0 – Tear out idea submission (external agencies cannot submit ideas; maintainer creates projects directly)
- [ ] Explore all Idea/submission references (`Idea.razor`, Submit flow, `ProjectManagerService.AddAsync`) and map what removal breaks.
- [ ] Remove external agency submit surface so new ideas cannot be created by agencies; decide with reviewer whether existing submitted data stays read-only or is removed.

### Phase 1 – Bug fixes: test first exposed in unit tests, then fix
(reprioritized from both lists after teardown)
- [ ] ProjectTable pagination dead (page never advanced); wire page advance + total range ("Showing 1–10 of N").
- [ ] Status alphabetical sort buries "New" last → pending-first ordering / filter defaulting to New.
- [ ] Filter box below table; move above and add submitter contact column/filter if model supports it.
- [ ] Authorization gaps: review actions reachable anonymously despite identity configured in Startup — guard list/actions or show explicit "log in to review" prompt.
- [ ] Relative login links 404 on nested routes → route-rooted/absolute URLs.

### Phase 2 – Security concerns
- [ ] `/download` endpoint unauthenticated (exports contain submitter PII) — add authorization.
- [ ] Comment deletion without ownership check in `CommentEntry`.
- [ ] Anonymous comment contact data silently dropped before `AddComment`.

### Phase 3 – Links generalization & attachments categories
- [ ] Generalize hardcoded `Url`/`Github`/`VideoLink` fields to N:any links via Enum + type-specific behavior handled as required; keep attachment category mapping separate (details my call).

### Phase 4 – Student Review, management and faculty aggregation by cohort / group by team — unravel sub-app duplication.
