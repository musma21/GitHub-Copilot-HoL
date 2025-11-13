# PySide6 Firmware Updater 프로젝트 지침서
버전: 1.0-PyQt-Adapt (2025-11-05)

## 1. 프로젝트 개요
이 프로젝트는 GIPAM Firmware Updater 애플리케이션을 PySide6 기반으로 구현한 것입니다. 
핵심 목적:
- 펌웨어 파일 선택, 크기 표기, CRC-16 체크섬 계산, 프로그램 업데이트 버튼 클릭 시 파일 전송 시작
- 진행률(실제 파일 청크 읽기) 표시 기능

## 2. 아키텍처 구성
| 영역 | 파일/컴포넌트 | 설명 |
|------|---------------|------|
| UI 메인윈도 | `ui/main_window.py` | 레이아웃, 스타일, 진행률 스레드, 로그 패널 |
| 파일 서비스 | `services/firmware_update.py` | 파일 크기 / SHA256 계산 (스트리밍) |
| 스타일시트 | `ui/style.qss` | 라이트 테마 기본 스타일 |
| 실행 스크립트 | `run_app.ps1` | 가상환경 생성/활성 + 앱 실행 + 자동 종료 옵션 |
| 측정 도구 | `measure_layout.py` | 레이아웃 기하 정보 / 스크린샷 캡처 |

## 3. 실행 방법
PowerShell (Windows):
```powershell
powershell -ExecutionPolicy Bypass -File .\run_app.ps1
```
자동 종료 (예: 8초 후):
```powershell
powershell -ExecutionPolicy Bypass -File .\run_app.ps1 -AutoCloseSeconds 8
```
환경 변수 직접 지정:
```powershell
$env:FWU_AUTOCLOSE_SECONDS = 5; powershell -ExecutionPolicy Bypass -File .\run_app.ps1
```

## 4. 진행률 & 스레드 모델
`FileProgressWorker` (QThread 내부 실행):
- 청크 단위(64KB)로 파일을 읽으며 누적 바이트/전체 바이트 비율 → % 이벤트 신호
- 인위적 pacing (QThread.msleep(25)) 으로 UI 갱신 시각적 안정성 확보
- 취소 시 `request_cancel()` → 즉시 루프 탈출, `cancelled` 신호 송출
중대한 변경 조건:
- Worker 시그니처(초기화 파라미터, 신호 세트) 변경
- 청크 크기 로직 및 진행률 계산 방식 변경 (예: 다른 백분율 공식)

## 5. UI 스타일 & 레이아웃 규칙
- UI의 크기와 배치는 다음의 그림을 참조하여 동일하게 구현할 것
![alt text](image.png)

## 6. 성능 기준
- 스타트업 빌드(초기 `_build_ui` + 스타일 적용) 20ms 이하 목표 (현재 ≈14–17ms)
- 진행률 업데이트 시 메인 스레드 UI 렌더 지연 >50ms 발생 시 재검토
- 파일 해시 계산은 Python 기본 hashlib 사용 (C 구현으로 충분히 빠름) → 별도 최적화는 대용량 >1GB 파일에서만 고려

## 7. 테스트 기준
각 변경 Pull 수준:
- 정상 케이스: 작은 임시 파일 진행률 0→100%
- 에지 케이스: 빈(0바이트) 파일 → 즉시 100% 완료
- 실패 케이스: 존재하지 않는 파일 선택 시 오류 처리
보고 형식:
- Build / Lint / Test: PASS or FAIL (실패 시 경로+요약)
공용 인터페이스 변경 시 README 또는 본 지침서에 마이그레이션 노트 추가

## 8. 로깅 패널 사용 원칙
- 시간(HH:MM:SS) 프리픽스 유지
- 오류 발생 시: `[시간] Error: 메시지` 형태
- 진행률 100% 도달 후 최종 로그 `Update completed` 출력
- 로그 자동 스크롤 (QTextCursor.End) 유지 — 변경 시 사전 승인

## 9. 보안 & 무결성
- 파일 존재 여부 검사 후만 크기/해시 계산
- 경로 외부 입력(네트워크/IP)은 수정 불가(ReadOnly)로 유지
- 민감 정보(비밀키/토큰) 저장 금지
중대한 보안 변경:
- 해시 알고리즘 교체 (SHA256 → 다른 알고리즘)
- 외부 네트워크 호출 추가
- 실행 스크립트 권한/정책 변경 (ExecutionPolicy 등)

## 10. 국제화(I18N) 로드맵
- 1단계: 현재 한국어 UI 유지, 내부 키 추출 준비
- 2단계: 문자열 리소스 모듈(`i18n/strings_ko.py`, `i18n/strings_en.py`) 도입
- 3단계: 런타임 언어 전환 메뉴 추가 (`English` 라벨 실제 동작 연결)
규칙:
- 플레이스홀더 포맷 통일 (예: `{filename}`)
- 다국어 추가 전 영어 기준 문구 확정

## 11. 변경 승인 필요 항목
사용자 사전 확인이 필요한 변경:
- 새로운 Python 패키지 추가 (pyproject.toml 수정)
- Worker 신호/메서드 시그니처 변경
- 스타일 전반 교체 (테마 구조 재작성)
- 실행 스크립트(`run_app.ps1`) 동작 방식 근본 변경 (예: 패키징 배포 전환)
- 자동 종료 로직 정책 변경 (환경 변수 이름 수정 등)

## 12. 피해야 할 것
- 불필요한 거대 프레임워크 도입 (예: 전체 DI 컨테이너)
- 조기 최적화 (측정치 없는 Thread 동시성 확대)
- 광범위한 try/except 후 무시
- UI와 실제 기능(진행률/취소) 불일치 방치

## 13. 문제 해결 프로토콜
드리프트 징후: 레이아웃 측정치(Row spread) 증가, 스타트업 시간 급증, 해시 계산 오류 로그 반복
조치:
1) 최근 변경 diff 재검토 → 2) 측정 스크립트 재실행 → 3) 원복 또는 미세 조정 후 로그 남김
우선순위 충돌 시: 보안 > 무결성 > UI 정확성 > 성능

## 14. 용어 정리
- 진행률 워커: 파일을 청크로 읽어 % 신호를 내보내는 객체
- 스타트업 빌드 시간: 창 생성부터 첫 paint 직전까지 측정한 ms
- Row spread: 동일 행 위젯들의 y 좌표 최대-최소 차이(px)
- 파리티(parity): WPF 대비 시각/기능적 동등성

## 15. 응답 프로토콜
중대한 변경 정의:
- 고유 수정 파일 > 5개
- `FileProgressWorker` 신호 또는 메서드 변경
- 스타일시트 전면 재작성 or PRIMARY_COLOR 변경
- 실행 경로(스크립트/모듈) 구조 재편
중대한 변경 시: 계획 + 영향 + 보안/성능 평가 → 승인 후 적용. 사소한 변경은 즉시 Diff 적용.
항상 마지막에 Build / Test 상태 보고.

## 16. 향후 개선 목록 (Roadmap)
- 드래그앤드롭 파일 선택
- 진짜 펌웨어 전송 채널 연계 (시뮬레이션 제거)
- 다국어 리소스 모듈화
- 진단 로그 저장(파일 rolling) + UI tail 뷰
- 진짜 취소 시 중단 지점 재개(Resume) 기능

## 17. 현 상태 메트릭(샘플)
- Startup UI build: ~14–17 ms
- Row1 vertical spread: 1 px
- 진행률 워커: 64KB 청크 / 인위적 25ms pacing

최종 업데이트: 2025-11-05