# Customizing GitHub Copilot Responses

> 더 상세한 배경과 원본 예시는 [customizing-GHCP-response-extended.md](customizing-GHCP-response-extended.md) 문서를 참고하세요. 아래는 이 Hands-on Lab에서 실제 사용되는 문서/파일들의 빠른 링크입니다.
>
> 언어 버전: [KO](customizing-GHCP-response-ko.md) · [EN](customizing-GHCP-response-en.md) · [Extended](customizing-GHCP-response-extended.md)
>
> - Repository-wide: [`.github/copilot-instructions.md`](../../.github/copilot-instructions.md)
> - Path-specific: [`MockWinAppInstaller.instructions.md`](../../.github/instructions/MockWinAppInstaller.instructions.md), [`MockWinAppInstaller.testing.instructions.md`](../../.github/instructions/MockWinAppInstaller.testing.instructions.md), [`MockWinAppInstaller.pitfalls.instructions.md`](../../.github/instructions/MockWinAppInstaller.pitfalls.instructions.md), [`MockWinAppInstaller.coding-convention.instructions.md`](../../.github/instructions/MockWinAppInstaller.coding-convention.instructions.md)
> - Agents: [`AGENTS.md`](../../AGENTS.md)
> - Prompt files 위치: [`.github/prompts/`](../../.github/prompts/)

짧은 요약:

- 세 가지 축: Repository Instructions / Path-Specific / Agent Instructions / Prompt Files.
- 목적: 반복 설명 줄이고 아키텍처·규칙·패턴을 모델이 일관되게 따르게 하기.
- 가독성과 유지보수: 간결(≤600 words repo-wide), 명확(Imperative style), 중복 제거.

## 1. Types of Customization

| Type | Scope | Injection | Typical Use |
|------|-------|-----------|-------------|
| Repository Instructions (`.github/copilot-instructions.md`) | 전체 코드베이스 | 자동 | 아키텍처, 빌드, 금지 패턴 |
| Path-Specific (`.github/instructions/*.instructions.md`) | 특정 디렉터리 | 자동 (우선순위 높음) | 서브모듈 규칙, 테스트 방침 |
| Agent Instructions (`AGENTS.md`) | 역할/스타일 | 자동 (VSCode Chat, 설정 필요) | git-mini, code-mini 등 역할별 응답 형태 |
| Prompt Files (`*.prompt.md`) | 개별 요청 모음 | 수동(선택/재사용) | 반복 작업(리팩터, 테스트 추가 등) |

> Prompt Files: VSCode/JetBrains (Public Preview). Command Palette 또는 `/파일명` 으로 호출.

## 2. Repository Instructions

작성 원칙:

- 맨 앞 3–6줄: 언어/프레임워크/빌드/주요 목표.
- 핵심 명령: install → build → test 순서.
- “Always” / “Must” 접두사로 필수 단계 표시.
- 금지/허용 패턴 명확 (예: “No blocking .Result / .Wait”).

설정:

1. 루트에 `.github/copilot-instructions.md` 추가.
2. VSCode 설정: `github.copilot.chat.codeGeneration.useInstructionFiles` 활성.
3. Chat에서 참조: `#file:.github/copilot-instructions.md`.

## 3. Path-Specific Instructions

목적: 하위 모듈 특수 규칙(예: WPF MVVM / 테스트 요구사항) 오버레이.
예시 구조:

```text
.github/instructions/
  MockWinAppInstaller.instructions.md          # 기본
  MockWinAppInstaller.testing.instructions.md  # 테스트 전략
  MockWinAppInstaller.pitfalls.instructions.md # 재발 방지
  MockWinAppInstaller.coding-convention.instructions.md
```
\n우선순위: Path-Specific > Repository-Wide (해당 경로 내).

## 4. Agent Instructions (AGENTS.md)

설정:

- VSCode: `chat.useAgentsMdFile`, `chat.useNestedAgentsMdFiles` 활성.
- 루트 `AGENTS.md` (Nested 다수는 Preview 기능).

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

사용: Chat에서 `@git-mini`, `@code-mini` 등 직접 언급.


## 5. Prompt Files

위치: 기본 `.github/prompts/`; 설정으로 추가 경로 확장 (`chat.promptFilesLocations`).
생성 후 사용 방법:

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

## 6. Syncing VSCode Settings Across Devices

VSCode Settings Sync 활성 → Instruction/Prompt 관련 설정 자동 동기화.
(이미지: VSCode Settings Sync UI)

![VSCode Settings Sync UI](assets/settings-sync-enable.png "Settings Sync enable screen")

## 7. Best Practices

### What Goes Where

| Put in Instructions | Put in Prompt File |
|---------------------|--------------------|
| 규칙·아키텍처·필수 빌드 단계 | 반복 실행 요청 (리팩터, 테스트 생성 등) |
| 금지 라이브러리 / 패턴 | 특정 파일 단위 변경 지시 |
| 프로젝트 디렉터리 개요 | 자주 쓰는 분석/검증 명령 |
| 테스트 범위 전략 | 한 번에 실행 가능한 단일 목적 작업 |

### Length Guidelines

- Repo Instructions: ≤600 English words 또는 ≤2,000 Korean chars.
- Prompt File: 유지보수 손쉬운 소형 컬렉션.

### Style

- 짧고 명령형: “Prefer async/await; avoid blocking waits.”
- 중립 톤 → 필요 시 사용자 요청에 따라 변형.
- 중복 자체 설명(“이 문서는 ~”) 최소화.

### Maintenance Checklist

- [ ] 링크 유효성 (404 제거)
- [ ] 이미지 alt 텍스트 존재
- [ ] 중복/타이포 제거 (`prompth`, `consisitency`)
- [ ] 금지 항목 최신화
- [ ] Prompt 파일 라인 수 ≤30
- [ ] 불필요한 HTML 엔티티(`&#x20;`) 없음

## 8. Examples (Good vs Improved)

### Good (Before)

“React components: function components only (no class).”

### Improved Pattern

```text
React: function components only.
Imports: use absolute path (no ../../../).
Async: use async/await; avoid raw Promise chains.
```

### Bad → Reason → Rephrase

| Bad | 문제 | Rephrase |
|-----|------|----------|
| “Answer all questions in informal style.” | 톤 강제 → 정확성 저하 | “Default: concise technical tone. Switch to casual only if user asks.” |
| “Use @terminal for Git.” | 조건 없음, 토큰 낭비 | “If user asks for a command (contains ‘run’/‘command’), then show Git CLI.” |
| “Always conform to styleguide.md in my-repo.” | 과다/불명확 범위 | 구체 bullet로 분해 (위 예처럼). |

## 9. Common Pitfalls (Copilot Context)

| Pitfall | Mitigation |
|---------|------------|
| Instruction 길이 초과로 핵심 누락 | 글자 수 제한 준수 + 핵심 우선 |
| Prompt 파일에 문단형 장문 | 한 줄 한 작업으로 재구성 |
| 해시형 이미지 파일명 의미 없음 | 의미 기반 파일명으로 교체 |

> 긴 지침(예: 15줄 이상 서술형)이 불가피할 경우 그대로 넣기보다 목적별로 작게 쪼개어 여러 짧은 bullet/문장으로 나누면 모델이 요약 손실 없이 더 안정적으로 따릅니다. 예: “테스트 전략” 장문의 문단 → “경계값 포함”, “부정 케이스 최소 1개”, “비동기 취소 시나리오 추가” 처럼 분할.

## 10. References

- About customizing Copilot responses  
  <https://docs.github.com/en/enterprise-cloud@latest/copilot/concepts/prompting/response-customization>
- Customization examples library  
  <https://docs.github.com/en/enterprise-cloud@latest/copilot/tutorials/customization-library/custom-instructions>
- Best practices for using GitHub Copilot  
  <https://docs.github.com/en/enterprise-cloud@latest/copilot/tutorials/coding-agent/get-the-best-results>
- Adding repository custom instructions  
  <https://docs.github.com/en/enterprise-cloud@latest/copilot/how-tos/configure-custom-instructions/add-repository-instructions>
- Soeun Park guide (Korean)  
  <https://cdn.microbiz.ai/public/GHE/github-copilot-instructions.md-guide.pdf>

