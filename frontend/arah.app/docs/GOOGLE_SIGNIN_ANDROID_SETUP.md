# Login com Google Sign-In no Android (passo a passo)

Este guia configura o login com Google + Firebase Auth no app Flutter para rodar **localmente** no emulador ou dispositivo via USB (debug). Package name: **com.Arah**.

---

## 1. Obter o SHA-1 da debug.keystore

O Google Cloud e o Firebase usam o SHA-1 do certificado para associar o app Android ao seu projeto.

### 1.1 Onde fica a debug.keystore

No Windows, a chave de debug do Android fica em:

```
%USERPROFILE%\.android\debug.keystore
```

Exemplo: `C:\Users\SEU_USUARIO\.android\debug.keystore`

### 1.2 Obter o SHA-1 via keytool (PowerShell ou CMD)

Abra o terminal e execute:

```powershell
keytool -list -v -keystore "$env:USERPROFILE\.android\debug.keystore" -alias androiddebugkey -storepass android -keypass android
```

**Saída esperada:** procure a linha **SHA1:** e copie o valor (ex: `A1:B2:C3:...`).

- Se não existir `debug.keystore`, rode primeiro um build Android do Flutter na pasta do app:  
  `cd frontend\Arah.app` e `flutter build apk --debug` (ou conecte um device e rode `flutter run`); o Android SDK criará a keystore na primeira vez.

### 1.3 Alternativa: SHA-1 pelo Gradle

Na pasta do app (com `android/` já criado):

```powershell
cd frontend\Arah.app
.\android\gradlew -p android signingReport
```

Na seção `Variant: debug`, copie o **SHA-1** de `Config: debug`.

---

## 2. Google Cloud Console – OAuth consent screen e Client ID Android

### 2.1 Criar ou selecionar projeto

1. Acesse [Google Cloud Console](https://console.cloud.google.com/).
2. Crie um projeto novo ou selecione um existente (ex: "Arah").

### 2.2 Configurar tela de consentimento OAuth

1. Menu **APIs e serviços** → **Tela de consentimento OAuth**.
2. Se for a primeira vez, escolha **Externo** (para testar com qualquer conta Google).
3. Preencha:
   - **Nome do app:** Arah
   - **E-mail de suporte:** seu e-mail
   - **Domínios autorizados:** em branco para dev
4. Salve.

### 2.3 Criar credencial “ID do cliente OAuth” (Android)

1. Menu **APIs e serviços** → **Credenciais**.
2. **+ Criar credenciais** → **ID do cliente OAuth**.
3. **Tipo de aplicativo:** **Android**.
4. Preencha:
   - **Nome:** Arah Android (debug)
   - **Nome do pacote:** `com.Arah`
   - **Impressão digital do certificado SHA-1:** cole o SHA-1 obtido no passo 1 (ex: `A1:B2:C3:...`).
5. **Criar**. Anote o **ID do cliente** (não é necessário colar no app Android quando usar Firebase + google-services.json; o Firebase usa esse Client ID automaticamente).

---

## 3. Firebase – Projeto e google-services.json

### 3.1 Criar projeto no Firebase (ou usar o mesmo do Google Cloud)

1. Acesse [Firebase Console](https://console.firebase.google.com/).
2. **Adicionar projeto** (ou use o projeto já vinculado ao Google Cloud).
3. Se pedir, ative o **Google Analytics** (opcional para dev).

### 3.2 Registrar o app Android no Firebase

1. No projeto Firebase, clique no ícone **Android** (Adicionar app).
2. **Nome do pacote Android:** `com.Arah`.
3. **Apelido do app (opcional):** Arah.
4. **Certificado de depuração SHA-1:** cole o **mesmo SHA-1** do passo 1.
5. **Registrar app**. Não é necessário preencher o “SDK do Firebase” manualmente no passo seguinte; vamos fazer pelo Flutter.

### 3.3 Baixar google-services.json

1. Na etapa “Baixar google-services.json”, clique em **Baixar google-services.json**.
2. Coloque o arquivo em:
   ```
   frontend/Arah.app/android/app/google-services.json
   ```
   (crie a pasta `android/app` se não existir; se o projeto ainda não tiver `android/`, veja a seção “Estrutura Android” no final.)

### 3.4 Ativar Authentication (Sign-in com Google)

1. No Firebase, menu **Build** → **Authentication**.
2. **Começar** (se for a primeira vez).
3. Aba **Sign-in method** → **Google** → **Ativar** e salve (pode usar o e-mail de suporte do projeto como “E-mail de suporte do projeto”).

---

## 4. Estrutura Android e Gradle (Flutter)

Se a pasta `android/` ainda não existir no app, crie a estrutura Android e depois ajuste os Gradle:

```powershell
cd frontend\Arah.app
flutter create . --platforms=android
```

**Importante:** o package name deve ser exatamente `com.Arah`. Em `android/app/build.gradle`, em `defaultConfig`, defina `applicationId "com.Arah"`. Se o template tiver outro valor (ex.: `com.example.araponga_app`), altere para `com.Arah` para coincidir com o Firebase e o Google Cloud. Depois ajuste os arquivos abaixo.

### 4.1 android/build.gradle (raiz do Android)

Adicione o plugin do Google Services na seção `dependencies`:

```gradle
buildscript {
    repositories {
        google()
        mavenCentral()
    }
    dependencies {
        classpath 'com.android.tools.build:gradle:7.4.2'   // ou a versão que o Flutter já tiver
        classpath 'com.google.gms:google-services:4.4.0'
        classpath "org.jetbrains.kotlin:kotlin-gradle-plugin:$kotlin_version"
    }
}
```

(Se já existir `classpath 'com.google.gms:google-services:...'`, não duplique.)

### 4.2 android/app/build.gradle

- **minSdkVersion:** pelo menos **23** (firebase-auth 23.x exige; template Flutter costuma usar 21).

Exemplo da parte relevante:

```gradle
android {
    namespace "com.Arah"
    compileSdkVersion 34

    defaultConfig {
        applicationId "com.Arah"
        minSdkVersion 23
        targetSdkVersion 34
        versionCode 1
        versionName "1.0"
    }
    ...
}
```

No **final do arquivo** (após o bloco `android { ... }`), adicione:

```gradle
apply plugin: 'com.google.gms.google-services'
```

### 4.3 Verificar android/settings.gradle

Garanta que há repositórios `google()` e `mavenCentral()` (o template Flutter já costuma ter).

---

## 5. Flutter – Dependências e inicialização

No `pubspec.yaml` já devem estar (ou adicione):

```yaml
dependencies:
  firebase_core: ^3.8.0
  firebase_auth: ^5.3.1
  google_sign_in: ^6.2.2
```

Depois:

```powershell
cd frontend\Arah.app
flutter pub get
```

No `main.dart`, inicialize o Firebase **antes** de `runApp`:

```dart
import 'package:firebase_core/firebase_core.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await Firebase.initializeApp();
  runApp(
    const ProviderScope(
      child: ArahApp(),
    ),
  );
}
```

(O código do app já faz isso no `main.dart` atualizado.)

---

## 6. Fluxo de login no app (resumo)

1. Usuário toca em **Entrar com Google**.
2. **Google Sign-In** abre a tela de contas Google.
3. Após escolher a conta, o app obtém `idToken` / `accessToken` e faz **Firebase Auth** `signInWithCredential(GoogleAuthProvider.credential(...))`.
4. O app envia os dados (Firebase UID ou Google ID, e-mail, nome) para o **BFF** `auth/social` e guarda o token do backend.
5. Na **aba Perfil**, o nome e o e-mail do usuário logado vêm do BFF (`me/profile`) e são exibidos na tela (e, se quiser, também do `currentUserProvider` da sessão).

---

## 7. Rodar localmente (emulador ou USB)

1. Emulador: abra um AVD e execute:
   ```powershell
   cd frontend\Arah.app
   flutter run
   ```
2. Celular via USB: ative a depuração USB, conecte e:
   ```powershell
   flutter run
   ```
3. Para apontar o app para o BFF local (ex.: sua máquina na rede):
   ```powershell
   flutter run --dart-define=BFF_BASE_URL=http://10.0.2.2:5001
   ```
   (use `10.0.2.2` no emulador para localhost do PC; no dispositivo físico use o IP da sua máquina, ex. `http://192.168.1.10:5001`.)

---

## 8. Checklist rápido

- [ ] SHA-1 da `debug.keystore` obtido e colado no Google Cloud e no Firebase.
- [ ] Credencial **OAuth Client ID** do tipo **Android** criada no Google Cloud (pacote `com.Arah`).
- [ ] App Android registrado no Firebase com o mesmo pacote e mesmo SHA-1.
- [ ] `google-services.json` em `android/app/google-services.json`.
- [ ] Plugin `com.google.gms.google-services` no `build.gradle` (raiz e app).
- [ ] `minSdkVersion 23` em `android/app/build.gradle` (firebase-auth 23.x exige).
- [ ] `Firebase.initializeApp()` no `main.dart`.
- [ ] Login com Google + Firebase Auth + chamada ao BFF implementados no repositório de auth.
- [ ] Nome/e-mail exibidos na tela de perfil (já coberto pelo fluxo BFF + `me/profile`).

Se algo falhar, confira: package name `com.Arah` em todos os lugares; SHA-1 igual no Cloud e no Firebase; e `google-services.json` na pasta correta.
