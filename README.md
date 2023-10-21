# CapsuleRolls

![capsule-rolls](https://github.com/johnhjh/CapsuleRolls-Releases/assets/59155657/b20544e2-3753-4da7-a8be-c7e56bc58f7b)

---

## 🎮 프로젝트 개요

### 수행 기간

- **2022.02.22 ~ 2022.04.17** (약 2 개월)

### CapsuleRolls 게임 소개

- **CapsuleRolls**는 모든 연령대의 유저가 즐길 수 있는 캐주얼 게임입니다. 남녀노소 누구나 즐길 수 있는 심플한 조작과 동시에 전략적인 컨트롤이 필요한 특성을 가지고 있어, 라이트 유저부터 헤비 유저까지 넓은 유저층을 대상으로 하고 있습니다. 개성 넘치는 캐릭터 커스터마이징과 다양한 게임 모드 (싱글 스테이지, 아케이드)를 제공하여 플레이어들에게 계속해서 새로운 도전과 재미를 선사합니다. 현재 PC (Windows)와 모바일 (Android)에서의 플레이가 가능합니다.

### 수행 역할

- **CapsuleRolls**는 1인 개발 프로젝트로, 게임의 전체적인 컨셉부터 세부적인 시스템까지 모두 혼자 직접 기획 및 구현하였습니다.
- 게임 기획 (컨셉, 시스템, 화면 등)
- 게임 디렉팅 (아트, 사운드 등)
- 게임 레벨 디자인
- 게임 개발 (클라이언트, 시스템 등)
- 에셋 수집 및 관리

### 설치 및 플레이

- [Capsule Rolls Releases](https://github.com/johnhjh/CapsuleRolls-Releases)

---

## 🛠 개발 환경

- **개발 툴**:

  - **Unity** (ver.2019.4.32f1)
  - **Microsoft Visual Studio Community 2019** (ver.16.11.9)
  - **Microsoft .NET Framework** (ver.4.8.040.84)
  - **MAST** (Modular Asset Staging Tool)
  - **GitHub**

- **기타 툴**:

  - **Blender**
  - **Adobe Photoshop**
  - **VSDC Video Editor**

- **개발 및 테스트 환경**:
  - **PC (Windows)** : Microsoft Windows 10 Pro x64 기반
  - **Mobile (Android)** : Samsung Galaxy S9+

---

## 🌟 핵심 기술

### Animator with Mecanim

- **Layers**: CapsuleRolls의 캐릭터 움직임은 다양한 레이어로 구성됩니다. `Base`, `Arm`, `Jump`, 그리고 `Emotion` 레이어를 통해 움직임의 우선도를 부여하였습니다. 각 레이어는 특정 동작에 초점을 맞추고 있어, 더욱 자연스러운 애니메이션을 구현할 수 있었습니다.

  ![layer](https://github.com/johnhjh/CapsuleRolls/assets/59155657/5790d93d-d2b7-4d72-b7c7-2d014fbe115b)

- **Sub-State Machine**: 복잡한 애니메이션 시퀀스를 효율적으로 관리하기 위해 Sub-State machine을 도입했습니다. 여러 애니메이션을 연속적으로, 그리고 원활하게 수행할 수 있도록 구현하였습니다.
  
  ![sub-state-machine](https://github.com/johnhjh/CapsuleRolls/assets/59155657/0ca82fde-7fa1-42ac-af61-05b251853dde)

- **Avatar Mask**: 캐릭터의 상체와 하체 애니메이션을 독립적으로 수행할 수 있게 하기 위해 Avatar Mask를 적용하였습니다. 이를 통해 더욱 다양한 캐릭터 움직임 조합을 구현하였습니다.
  
  ![avatar-mask](https://github.com/johnhjh/CapsuleRolls/assets/59155657/0caf4732-ee59-4ef3-9475-dfc8446cc08f)


- **Blend Trees**: Blend Trees를 적용하여 다양한 애니메이션 상태 간의 전환을 부드럽게 만들었습니다. 캐릭터의 움직임이나 포즈 변경 시 자연스러운 연출을 위해 사용한 기술입니다.
  
  ![blend-trees](https://github.com/johnhjh/CapsuleRolls/assets/59155657/1dc48adf-c92e-4445-bca5-dbc1854f964f)


---
