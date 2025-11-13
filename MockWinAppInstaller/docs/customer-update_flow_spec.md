# 펌웨어 업데이트 v2 사양 (Draft)

## 1. 개요
기존에는 `업데이트 실행` 버튼 클릭 시 선택된 펌웨어 파일을 로컬 임시 경로(`C:\Users\user\Desktop\Copilot Test\temp`)로 단순 복사하고(선택적으로 CRC16 검증) 완료 팝업을 띄우는 흐름이었습니다. 본 문서는 **새로운 업데이트 처리 방식(v2)** 에 대한 요구사항과 구현/검증 기준을 정의합니다.

## 2. 기존 동작 요약
| 항목 | v1 동작 |
|------|---------|
| 저장 위치 | 로컬 temp 폴더로 복사 |
| 진행 표시 | 단순 파일 복사 퍼센트 (0~100) |
| 체크섬 | CRC16 (체크박스 ON일 때만) |
| 실패 처리 | 에러 문자열 팝업 1회 |
| 로그 | 콘솔 출력 위주 |

## 3. 변경 목표 (v2)
| 항목 | 목표 |
|------|------|
| 저장/전송 방식 | (예) 디바이스/네트워크/API 업로드 또는 지정 프로토콜 수행 |
| 단계화 | 준비 → 전송 → 검증 → 완료 (UI에 단계 표시) |
| 체크섬 전략 | 옵션: CRC16 / Skip |
| 검증 항목 | 크기(Size) + CRC16 동일성 (+ 선택적 추가 해시) |
| 에러 가시성 | 코드/원인/사용자 메시지 + 재시도 버튼 |
| 성능 지표 | MB/s, 경과 시간, 예상 남은 시간 표시 |
| 국제화 | 한/영 모두 명세 반영 |
| 확장성 | 향후: 서명(Signature) 검증, 압축/암호화 지원, 롤백(revert) |

## 4. 상세 기능 요구사항
### 4.1 UI 변경
- 진행 상태 그룹에 단계 라벨 추가: `준비중 / 전송중 / 검증중 / 완료`
- 속도/진행 정보: `전송 속도: X MB/s`, `남은 시간: mm:ss`
- 해시 선택 컴포넌트 (드롭다운): `None | CRC16`
- 체크박스 제거 대신: 드롭다운이 `None`일 때 해시 표시 영역은 `-` 유지
- 취소 버튼 추가 (전송 단계에서만 활성)

### 4.2 기능 흐름(시퀀스) - (Updated: TCP + MODBUS-RTU + FTP 요구사항 반영)
`업데이트 실행` 버튼 동작 요구사항이 로컬 복사에서 네트워크 기반 절차로 변경되었습니다. v2 기본 흐름은 다음 5개 상위 Phase로 구성됩니다.

1. (Connect) TCP 서버 접속 (포트 5432)
2. (Trigger-Begin) MODBUS-RTU 프레임 송신 (시작 트리거)
3. (Transfer) FTP 접속 및 펌웨어 파일 전송 (Main / Comm)
4. (Trigger-End) MODBUS-RTU 프레임 송신 (종료/적용 트리거)
5. (Verify & Finish) 선택적 검증(해시/크기) 및 완료 처리

세부 단계:

| Phase | 세부 단계 | 설명 | 실패 시 에러 코드 |
|-------|-----------|------|-------------------|
| Connect | TCP Connect | 대상 장비 TCP 서버에 5432 포트로 접속 (Raw 소켓) | E_TCP_CONNECT |
| Trigger-Begin | Send MODBUS Start | 시작 트리거 프레임 전송 (RTU Frame over TCP) | E_MODBUS_SEND |
| Transfer | FTP Connect/Login | FTP 서버 접속 및 로그인 (`admin` / `lsisbiz`) | E_FTP_CONNECT / E_FTP_LOGIN |
| Transfer | FTP Put Main | 선택된(또는 지정된) 메인 펌웨어를 `XGIPAM_Main.bin` 으로 rename 후 `B:\mainFW` 경로 업로드 | E_FTP_TRANSFER / E_FTP_RENAME |
| Transfer | FTP Put Comm | 통신 펌웨어를 `XGIPAM_Comm.bin` 으로 rename 후 `B:\commFW` 경로 업로드 | E_FTP_TRANSFER / E_FTP_RENAME |
| Trigger-End | Send MODBUS Apply | 적용 트리거 프레임 2종 전송 | E_MODBUS_SEND |
| Verify & Finish | Hash/Size Verify (옵션) | 선택된 해시 옵션에 따라 원본 vs 대상 검증 (가능한 경우) | E_HASH |

#### 4.2.1 MODBUS-RTU 프레임 사양 (RTU 프레임을 TCP 소켓에 그대로 전송)
로우 TCP 스트림 상에 MODBUS-RTU 프레임(주소 / 기능 / 데이터 / CRC Low / CRC High)을 그대로 전송합니다. 제공된 PDU들은 이미 CRC를 포함한 완전한 RTU 프레임(Byte sequence)입니다.

| 용도 | 프레임 (HEX 시퀀스) | 분석 | 비고 |
|------|--------------------|------|------|
| 시작 트리거 | F8 05 05 35 FF 00 88 91 | Addr=0xF8, Func=0x05(Write Single Coil), AddrHi=0x05, AddrLo=0x35 (0x0535=1333), 값=0xFF00(ON), CRC=0x9188 (전송 순서 Low=0x88 High=0x91) | Begin Update |
| 종료 트리거 1 | F8 05 04 C9 FF 00 49 5D | Addr=0xF8, Coil=0x04C9(1225) ON, CRC=0x5D49 | Apply Main + Comm 시작 1 |
| 종료 트리거 2 | F8 05 04 CA FF 00 B9 5D | Addr=0xF8, Coil=0x04CA(1226) ON, CRC=0x5DB9 | Apply Main + Comm 시작 2 |

전송 절차:
1. 소켓 연결 성공 후 1개 시작 프레임 전송 → 응답(optional) 수신 (장비 응답 형식 미정인 경우 Timeout 후 다음 단계 진행 가능 configurable)
2. FTP 전송 완료 후 종료 트리거 2종 순차 전송 (각 1 프레임)
3. 실패 시 재시도 횟수 (기본 1회) 후 에러 처리

#### 4.2.2 FTP 전송 사양
- 인증 정보: username=`admin`, password=`lsisbiz`
- 모드: Passive 모드 사용 (방화벽 친화성) (가능 시 자동 협상 / 실패 시 Active fallback 옵션화)
- 전송 파일:
    - 메인 펌웨어: 업로드 시 로컬 선택 파일을 `XGIPAM_Main.bin` 으로 rename → 원격 경로 `B:\mainFW\XGIPAM_Main.bin`
    - 통신 펌웨어: 업로드 시 로컬 선택 파일(또는 별도 지정)을 `XGIPAM_Comm.bin` 으로 rename → 원격 경로 `B:\commFW\XGIPAM_Comm.bin`
- 파일 선택 전략:
    - (단순모드) 하나의 입력 파일만 있는 경우: 동일 파일을 두 경로에 각각 다른 이름으로 업로드 (향후 UI 확장으로 separate file picker 지원 예정)
    - (확장모드) UI에서 Main/Comm 별도 선택 가능 시 각 파일을 해당 이름으로 업로드
- 무결성: 업로드 직후(가능하면) FTP SIZE 명령 또는 MLSD로 크기 확인. 해시 검증은 장비 측 해시 제공 기능이 없으면 클라이언트 측 원본 해시만 로그.
- 실패 처리: 네트워크 오류 / 로그인 실패 / 경로 없음 / rename 실패 별도 코드 매핑

#### 4.2.3 기존 해시/검증 단계와의 연계
스트리밍 전송 시 원본 파일 읽기와 동시에 CRC16 누적 가능. FTP 업로드는 라이브러리 수준에서 바이트 스트림 콜백을 통해 진행률 및 해시(CRC16) 업데이트 병행. 종료 트리거 송신 전 CRC16 계산이 완료되어야 함.

#### 4.2.4 취소 로직 (네트워크 흐름)
- Connect 이전: 즉시 중단
- Transfer 중: 현재 진행 중인 FTP 전송 abort, 부분 파일 삭제 시도 (FTP DELE) (실패해도 치명적 아님 - 로그 기록)
- Trigger-End 이전에 취소된 경우: 시작 트리거만 보낸 상태로 종료 → 장비 측 반쪽 상태 가능성 경고 로그

> NOTE: 장비 측 프로토콜이 MODBUS TCP (0x0000 Transaction Header) 가 아니라 RTU CRC 포함 프레임을 raw socket 으로 받는 것으로 명시됨. 추후 장비 사양 변경 시 이 섹션 업데이트 필요.

#### 4.2.5 연결 모드별 IP 규칙 (USB / Ethernet)
- USB 모드(`_connection_type == 'usb'`):
    - 로컬 PC 어댑터 "NGIPAM USB Ethernet Driver" IP를 `172.30.0.1/24` 로 설정 (netsh 시도, 실패 시 경고 로그만 기록).
    - 원격 장비(TCP 서버) IP는 고정 `172.30.0.33` 사용.
    - 사용자 입력 IP 필드는 무시(표시용은 'USB').
- Ethernet 모드(`_connection_type == 'ethernet'`):
    - 사용자 입력 IP(`_device_ip`)를 원격 TCP 서버 IP로 사용.
    - 별도 어댑터 설정 없음.
실패 시 재시도 없이 진행(USB 어댑터 설정 실패는 치명적 오류로 간주하지 않고 연결 시도 계속). 향후 필요 시 `E_ADAPTER_CONFIG` 코드 추가 가능.

### 4.3 워커 설계
- 추상 BaseWorker: `prepare()`, `transfer_loop()`, `finalize()`
- 구현 예: `LocalCopyWorker`, `DeviceUploadWorker`
- 공통 시그널: `progress(int)`, `phase(str)`, `speed(float)`, `error(code:str, message:str)`, `finished(success:bool)`

### 4.4 체크섬 처리
- 테이블 기반 CRC16 유지 (현재 구현)
- 스트리밍 누적: 전송 루프에서 원본 데이터 읽을 때 해시 동시 업데이트 → 사후 재읽기 제거
- 검증: 대상 측 해시 계산 필요 시 두 번째 경로 읽기 (가능하다면 대상 측에서 제공되는 해시 활용)

### 4.5 에러/예외 처리
| 코드 | 설명 | 사용자 메시지(ko) | 사용자 메시지(en) | 재시도 가능 |
|------|------|------------------|------------------|--------------|
| E_NO_FILE | 파일 미선택 | 파일을 먼저 선택하세요 | Please select a file first | Y |
| E_NOT_FOUND | 파일 없음 | 파일이 존재하지 않습니다 | File not found | N |
| E_READ | 읽기 오류 | 파일을 읽는 중 오류 | Error reading file | N |
| E_WRITE | 쓰기/전송 오류 | 전송 중 오류 발생 | Transfer error | Y |
| E_HASH | 체크섬 불일치 | 체크섬이 일치하지 않습니다 | Checksum mismatch | Y |
| E_CANCEL | 사용자 취소 | 작업이 취소되었습니다 | Operation cancelled | Y |
| E_TCP_CONNECT | TCP 접속 실패 | TCP 서버 접속 실패 | TCP connection failed | Y |
| E_MODBUS_SEND | MODBUS 전송 실패 | MODBUS 프레임 전송 실패 | Modbus frame send failed | Y |
| E_FTP_CONNECT | FTP 접속 실패 | FTP 서버 접속 실패 | FTP connection failed | Y |
| E_FTP_LOGIN | FTP 로그인 실패 | FTP 인증 실패 | FTP authentication failed | Y |
| E_FTP_TRANSFER | FTP 전송 실패 | FTP 파일 전송 실패 | FTP file transfer failed | Y |
| E_FTP_RENAME | FTP 이름변경 실패 | FTP 파일 이름 변경 실패 | FTP file rename failed | Y |

### 4.6 성능 측정
- 기록: 총 시간(ms), 평균/최고 속도(MB/s), 해시 계산 시간 분리
- 로깅: `logs/update_YYYYMMDD_HHMMSS.log`
- 임계치 알림: 해시 계산 5초 이상이면 경고 표시 ("체크섬 계산이 오래 걸립니다")

### 4.7 국제화(i18n)
- 모든 UI 문자열은 기존 `TRANSLATIONS` 딕셔너리 확장
- 에러 코드별 한/영 메시지 매핑 테이블 추가

### 4.8 사용자 경험(UX)
- 전송 중 취소: 즉시 쓰레드 중단 + 부분 파일 삭제 + 상태 `Cancelled`
- 완료 팝업: 간단 모드(성공만) / 상세 모드(성능 요약) 토글 가능 (환경변수 `FWU_VERBOSE=1`)

### 4.9 보안/무결성(향후)
- 서명 검증(전자서명) 추가 가능성 -> 별도 `signature_enable` 옵션 고려
- 암호화된 패키지(.enc) 처리 시 복호 단계 삽입

## 5. 데이터 구조 초안
```python
class UpdateResult(TypedDict):
    success: bool
    phase: str
    size_bytes: int
    original_hash: str | None
    target_hash: str | None
    hash_algo: str  # 'none' | 'crc16'
    duration_ms: int
    avg_speed_mb_s: float
    peak_speed_mb_s: float
    error_code: str | None
    error_message: str | None
```

## 6. 완료 기준 (Definition of Done)
1. 해시 옵션 'None' 선택 시 해시 읽기/계산 추가 비용 없음
2. 전송 중 진행률/속도/남은 시간 모두 갱신
3. 취소 시 부분 파일 삭제 및 성공 팝업 미표시
4. 해시 선택 시 결과 값 대문자(HEX) 표준화
5. 에러 발생 시 코드 + 현지화 메시지 표시
6. 로그 파일 생성 및 최소: 시작/종료/에러/속도 지표 기록

## 7. 테스트 시나리오
| 시나리오 | 조건 | 기대결과 |
|----------|------|----------|
| S1 | 작은 파일 + None 해시 | 빠른 완료, 해시 표시 '-' |
| S2 | 큰 파일 + CRC16 | 진행률 자연 증가, 완료 후 CRC 일치 |
| S3 | 큰 파일 + CRC16 (비교) | 진행률 증가 후 CRC16 일치 |
| S4 | 중간에 취소 | 진행 중단, 파일 삭제, 취소 팝업 |
| S5 | 강제 오류(쓰기 권한 없음) | 에러 팝업 코드 E_WRITE |
| S6 | 체크섬 불일치 강제(인위적 변조) | 코드 E_HASH + 재시도 가능 |

## 8. 향후 확장 Backlog
- 서명 검증
- 병렬 전송 (분할 + 재조립)
- 자동 재시도 정책 (네트워크 실패)
- UI 다크/라이트 동적 테마 전환

## 9. 구현 우선순위 (MVP)
1. 해시 옵션 드롭다운 (None/CRC16)
2. 스트리밍 CRC16 누적 (재읽기 제거)
3. 속도/남은시간 계산 및 표시
4. 취소 버튼 + 안전 중단
5. 에러 코드/국제화 테이블
6. 로그 파일

---
초안(Draft) 문서입니다. 변경/추가 필요 시 위 섹션에 바로 편집 후 버전 태깅(`v2.0-spec`) 권장.