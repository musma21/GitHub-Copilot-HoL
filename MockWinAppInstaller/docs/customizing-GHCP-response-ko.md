# Customizing GitHub Copilot Responses

> 더 상세한 배경과 원본 예시는 [customizing-GHCP-response-extended.md](customizing-GHCP-response-extended.md) 문서를 참고. 아래는 이 Hands-on Lab에서 실제 사용되는 문서/파일들의 빠른 링크. (언어 버전: [KO](customizing-GHCP-response-ko.md) · [EN](customizing-GHCP-response-en.md))  
> - Repository-wide: [`.github/copilot-instructions.md`](../../.github/copilot-instructions.md)
> - Path-specific: [`MockWinAppInstaller.instructions.md`](../../.github/instructions/MockWinAppInstaller.instructions.md), [`MockWinAppInstaller.testing.instructions.md`](../../.github/instructions/MockWinAppInstaller.testing.instructions.md), [`MockWinAppInstaller.pitfalls.instructions.md`](../../.github/instructions/MockWinAppInstaller.pitfalls.instructions.md), [`MockWinAppInstaller.codeing-convention-instructions.md`](../../.github/instructions/MockWinAppInstaller.codeing-convention-instructions.md)
> - Agents: [`AGENTS.md`](../../AGENTS.md)
> - Prompt files 위치: [`.github/prompts/`](../../.github/prompts/)

짧은 요약:

- 세 가지 주요 지침 타입 + 프롬프트 파일 
  - 전역 지침 (Repository Instructions) 
  - 경로별 지침 (Path-Specific) 
  - 에이전트 지침 (Agent Instructions) 
  - 프롬프트 파일 (Prompt Files)
- 목적: 문맥 (Context)를 코파일럿에 제공하여 반복 설명 줄이고 아키텍처·규칙·패턴 등의 일관성 보장
- 가독성과 유지보수 
  - 간결 (영: 600 단어, 한: 2000자) : 토큰 소비 최소화
  - 명확 (지시적 명제 Imperative style), 중복 제거
> - 이 때 "가독성"은 GH Copilot의 입장에서의 가독성, 그러므로 코파일럿에 물어 이해할 수 있는 한 축약하는 것도 좋은 방법
> - 단, 신규입사자나 인수인계를 위해 사람을 위한 가이드가 필요할 경우, 별도의 문서를 제공할 수도 있음
  >> [AGENTS-commentary.md](AGENTS-commentary.md)<br/>
  >> [coding-conventions-commentary.md](coding-conventions-commentary.md)<br/>
  >> [copilot-instructions-commentary.md](copilot-instructions-commentary.md)<br/>
  >> [path-specific-instructions-commentary.md](path-specific-instructions-commentary.md):

## 1. 종류 (Types of Customization)

| Type | Scope | Injection | Typical Use |
|------|-------|-----------|-------------|
| 전역 지침 (Repository Instructions) (`.github/copilot-instructions.md`) | 전체 코드베이스 | 자동 | 아키텍처, 빌드, 금지 패턴 |
| 경로별 지침 (Path-Specific) (`.github/instructions/*.instructions.md`) | 특정 디렉터리 | 자동 (우선순위 높음) | 서브모듈 규칙, 테스트 방침 |
| 에이전트 지침 (Agent Instructions) (`AGENTS.md`) | 역할/스타일 | 자동 (VSCode Chat, 설정 필요) | git-mini, code-mini 등 역할별 응답 형태 |
| 프롬프트 화일 (Prompt Files) (`*.prompt.md`) | 개별 요청 모음 | 수동(선택/재사용) | 반복 작업(리팩터, 테스트 추가 등) |

> - 프롬프트 파일: VSCode/JetBrains에서만 (현재 Public Preview). Command Palette 또는 `/프롬프트 파일명` 으로 호출; (자동 주입 아님)
> - 왜 굳이 프롬프트 파일만 지침 (instructions)라고 부르지 않을까요? 🤔 (자동 주입 여부!)

## 2. 전역 지침 (Repository Instructions)

작성 원칙:

- 맨 앞 3–6줄: 언어/프레임워크/빌드/주요 목표.
- 핵심 명령: install → build → test 순서.
- “Always” / “Must” 접두사로 필수 단계 표시.
- 금지/허용 패턴 명확 (예: “No blocking .Result / .Wait”).

설정:

- 루트에 `.github/copilot-instructions.md` 추가.
- VSCode 설정: `⚙️github.copilot.chat.codeGeneration.useInstructionFiles` 활성화
  >  vscode://settings/github.copilot.chat.codeGeneration.useInstructionFiles
- Chat에서 참조: `#file:.github/copilot-instructions.md`.(수동 주입 방법)

## 3. 경로별 지침 (Path-Specific Instructions)

목적: 하위 모듈 특수 규칙(예: WPF MVVM / 테스트 요구사항) 오버레이.
예시 구조:

```text
.github/instructions/
  MockWinAppInstaller.instructions.md          # 기본
  MockWinAppInstaller.testing.instructions.md  # 테스트 전략
  MockWinAppInstaller.pitfalls.instructions.md # 재발 방지
  MockWinAppInstaller.coding-convention.instructions.md
```
- 우선순위: Path-Specific > Repository-Wide (해당 경로 내).

## 4. 에이전트 지침 (Agent Instructions) (AGENTS.md)

설정:

- VSCode: `chat.useAgentsMdFile`, `chat.useNestedAgentsMdFiles` 활성.
  > vscode://settings/chat.useAgentsMdFile vscode://settings/chat.useNestedAgentsMdFiles
- 루트 `AGENTS.md` (하위 경로에 있는 것도 Preview 기능으로 지원 중).

간단 예:

```markdown
Agent: git-mini
Scope: core git verbs & single-conflict fix. Behavior: terse 1–2 bash lines.

Agent: term-mini
Scope: glossary/acronym ≤3 sentences.

Agent: code-mini
Scope: micro code edits (≤15 changed lines, ≤2 files).

Agent: arch-pro
Scope: architecture, refactor, performance, security (structured output).
```

사용: Chat에서 `@git-mini`, `@code-mini`와 같이 멘션하고 명령.
> `@git-mini cmtpsh`<br/>
> : `cmtpsh`가 commit and push all local changes 라고 지침에 있을 경우, 간략한 수행 결과만 보고하도록 


## 5. 프롬프트 파일 (Prompt Files)

위치: 기본 `.github/prompts/`; 설정으로 추가 경로 확장 가능 (`chat.promptFilesLocations`).<br/>
생성 후 사용 방법 (즉, 수동 주입 방):

1. 에디터 상단 ▶ 버튼 클릭.
2. Chat 입력창 `/파일명` 자동 완성.
3. 명령 팔레트: “Chat: Run Prompt”.

작성 규칙:

- 한 줄 = 하나의 명령형 작업.
- 15–30 라인 유지, 50+ 라인 → 정리/아카이브.
- 길이: 각 라인 ≤160 chars (한국어 ≤100자).

예:

```text
Refactor #file:src/Services/ChecksumService.cs to use async hashing with cancellation.
Add tests for #folder:src/ViewModels focusing progress cancellation edge cases.
Explain DNP vs MODBUS protocol choice for firmware update (trade-offs).
```

## 6. 여러 개발 디바이스로 VSCode 설정 동기화

VSCode Settings Sync 활성 → Instruction/Prompt 관련 설정 자동 동기화 활성화 (아래 `Backup and Sync Settings...`).

![VSCode Settings Sync UI](assets/settings-sync-enable.png "Settings Sync enable screen")

## 7. 모범 사례 (Best Practices)

### 지침 (Intructions) vs 프롬프트 파일 (Prompt files)

| Instructions | Prompt File |
|---------------------|--------------------|
| 규칙·아키텍처·필수 빌드 단계 | 반복 실행 요청 (리팩터, 테스트 생성 등) |
| 금지 라이브러리 / 패턴 | 특정 파일 단위 변경 지시 |
| 프로젝트 디렉터리 개요 | 자주 쓰는 분석/검증 명령 |
| 테스트 범위 전략 | 한 번에 실행 가능한 단일 목적 작업 |

### Length Guidelines

- 지침 (Instructions): ≤600 English words 또는 ≤2,000 Korean chars.
- 프롬프트 파일 (Prompt File): 유지보수 손쉬운 소형 컬렉션.

### Style

- 짧고 명령형: “Prefer async/await; avoid blocking waits.”
- 중립 톤 → 필요 시 사용자 요청에 따라 변형.
- GH Copilot이 정확히 이해할 수 있으면 됨

### Maintenance Checklist

- [ ] GH Copilot에게 모범사례에 비추어 피드백을 받고
- [ ] GH Copilot이 이해할 수 있는 말로 축약하도록 하고
- [ ] GH Copilo에게 사람을 위한 가독성을 위해서는 별도 Commenatry 문서를 작성하게 하고
- [ ] GH Copilot에게 위 축약한 지침을 풀어서 설명하도록(rephrase) 하여 제대로 이해하고 있는지 사람이 최종 점검

## 8. 사례 

### Bad → Reason → Rephrase

| Bad | 문제 | Rephrase |
|-----|------|----------|
| “Answer all questions in informal style.” | 톤 강제 → 정확성 저하 | “Default: concise technical tone. Switch to casual only if user asks.” |
| “Use @terminal for Git.” | 조건 없음, 토큰 낭비 | “If user asks for a command (contains ‘run’/‘command’), then show Git CLI.” |
| “Always conform to styleguide.md in my-repo.” | 과다/불명확 범위 | 구체적 bullet로 분해 (위 예처럼). |

## 9. 참고자료 (References)

* [About customizing GitHub Copilot responses](https://docs.github.com/en/enterprise-cloud@latest/copilot/concepts/prompting/response-customization)
* [Curated collection of examples](https://docs.github.com/en/enterprise-cloud@latest/copilot/tutorials/customization-library/custom-instructions)
* [Best practices for using GitHub Copilot](https://docs.github.com/en/enterprise-cloud@latest/copilot/tutorials/coding-agent/get-the-best-results)
* [Adding repository custom instructions for GitHub Copilot](https://docs.github.com/en/enterprise-cloud@latest/copilot/how-tos/configure-custom-instructions/add-repository-instructions)
* [Copilot-Instruction 1-pager guide by Soeun Park@MS](https://cdn.microbiz.ai/public/GHE/github-copilot-instructions.md-guide.pdf) (Korean) 
* [Adding repository custom instructions for GitHub Copilot](https://docs.github.com/en/enterprise-cloud@latest/copilot/how-tos/configure-custom-instructions/add-repository-instructions)

