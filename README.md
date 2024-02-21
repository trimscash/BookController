# BookController
これは本にスイッチを取り付けてページをめくるたびにデータをBLEで送信しPC側で電子書籍をコントロールするやつ




https://github.com/trimscash/BookController/assets/42578480/10a6a2e6-d101-4621-abbe-29caedb26872

https://github.com/trimscash/BookController/assets/42578480/66c56bf5-ebf5-4bf0-8bf6-453832c29d21

https://github.com/trimscash/BookController/assets/42578480/71f39035-84b0-46e3-85a6-7387d97cc3cd


# 環境構築
```
git clone git@github.com:trimscash/BookController.git
```
でダウンロードできます．（学校WiFiからだと無理）

- /BookController

  マイコン用のプログラム

- /BookControllerApp

  MAUIのWindowsアプリのプログラムです

## BookController用の環境構築（マイコン
- VSCodeのインストール （https://azure.microsoft.com/ja-jp/products/visual-studio-code
- PlatformIOのインストール（https://zenn.dev/kotaproj/articles/esp32_vscode_pio

## BookControllerApp用の環境構築 （windowsアプリ
- VisualStudio2022をインストールしてください．(https://visualstudio.microsoft.com/ja/
- MAUI（フレームワーク）のインストールとチュートリアル(https://dotnet.microsoft.com/ja-jp/learn/maui/first-app-tutorial/intro

# 開発
コミットするときはブランチを新たに作ってください．

例えば新しい画面を実装したのならブランチ名は以下のようにしてください．
以下のように，機能を追加したのならば`add/`をつけてそのあとに`new_sceen`など追加した機能を英語でつけてください．
```
add/new_screen
```

修正したときはすべて`fix`をつけてください．
```
fix/new_sceen_bug
```
