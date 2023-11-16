# BookController
これは本にスイッチを取り付けてページをめくるたびにデータをBLEで送信しPC側で電子書籍をコントロールするやつ

# 環境構築
```
git clone git@github.com:trimscash/BookController.git
```
でダウンロードできます．（学校WiFiからだと無理）

## BookControllerはマイコン用のプログラム
BookControllerAppはMAUIのWindowsアプリのプログラムです

## BookController用の環境構築（マイコン
- VSCodeのインストール （https://azure.microsoft.com/ja-jp/products/visual-studio-code
- PlatformIOのインストール（https://zenn.dev/kotaproj/articles/esp32_vscode_pio

BookControllerApp用の環境構築 （windowsアプリ
- VisualStudio2022をインストールしてください．(https://visualstudio.microsoft.com/ja/
- MAUI（フレームワーク）のインストールとチュートリアル(https://dotnet.microsoft.com/ja-jp/learn/maui/first-app-tutorial/intro
