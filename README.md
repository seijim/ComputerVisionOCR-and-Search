# Computer Vision API (OCR) v2 と Azure Search を用いて、スキャンされた画像文書を検索＆プレビューできるようにしたサンプル WPF アプリケーション
アプリケーションの実行に当たっては、「WpfAppCvSearch」フォルダーのソリューションファイルを Visual Studio 2017 で開き、ビルド/リビルドしてください。ビルド時にエラーになる場合、一度、ソリューションのクリーンをしてから再度ビルドをしてください。ソースコードが不要な方には、バイナリも用意しておりますので、Installer.zip ファイルをダウンロードして解凍し、setup.exe を起動してインストールしてください。
<br/><br/>
## アプリケーション構成
<img src="./images/app00.png" /><br/>
<br/><br/>
## アプリケーションの実行
### 初期設定
アプリケーションを初めて実行する場合、以下の設定画面が最初に開きます。ここでは、<a href="https://westus.dev.cognitive.microsoft.com/docs/services/5adf991815e1060e6355ad44/operations/56f91f2e778daf14a499e1fc">Cognitive Services - Computer Vision API (OCR) V2</a> 用の設定情報、<a href="https://docs.microsoft.com/ja-jp/azure/search/search-what-is-azure-search">Azure Search</a> 用の設定情報、<a href="https://docs.microsoft.com/ja-jp/azure/storage/common/storage-introduction">Azure Storage</a> 用の設定情報を入力する必要があります。
<img src="./images/app01.png" width="70%"/><br/><br/>
最初に、Computer Vision API (OCR) V2 用のエンドポイントと API Key を入力します。
<img src="./images/app02.png" width="70%"/><br/><br/>
Azure Portal で、Computer Vision API を作成すると、以下の２つの画面からエンドポイントと API Key を取得できます。アプリケーションの設定画面には、ここで得られたエンドポイントの文字列に、"vision/v2.0/ocr" を加えて設定してください。 
<img src="./images/portal02-1.png" /><br/>
<img src="./images/portal02-2.png" /><br/><br/>

<img src="./images/app03.png" width="70%"/><br/>
<img src="./images/portal03-1.png" /><br/>
<img src="./images/portal03-2.png" /><br/>
<img src="./images/portal03-3.png" /><br/>
<img src="./images/app04.png" width="70%"/><br/>
<img src="./images/portal04-1.png" /><br/>
<img src="./images/portal04-2.png" /><br/>
<img src="./images/app05.png" width="70%"/><br/>
<img src="./images/app06.png" /><br/>
<img src="./images/app07.png" width="70%"/><br/>
<img src="./images/app08.png" width="70%"/><br/>
<img src="./images/portal08-1.png" /><br/>
<img src="./images/app11.png" /><br/>
<img src="./images/app12.png" width="70%"/><br/>
<img src="./images/app13.png" /><br/>
<img src="./images/app14.png" /><br/>
<img src="./images/app15.png" /><br/>
<img src="./images/app16.png" /><br/>
<img src="./images/app17.png" width="70%"/><br/>
<img src="./images/app18.png" /><br/>
<img src="./images/app19.png" width="70%"/><br/>
<img src="./images/app20.png" /><br/>
<img src="./images/app21.png" /><br/>
<img src="./images/app22.png" /><br/>
<img src="./images/app23.png" /><br/>
