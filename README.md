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
<a href="https://portal.azure.com/">Azure Portal</a> で、AI + Machine Learning カテゴリーにある Cognitive Services - Computer Vision を作成すると、以下の２つの画面からエンドポイントと API Key を取得できます。アプリケーションの設定画面には、ここで得られたエンドポイントの文字列に、"vision/v2.0/ocr" を加えて入力してください。 
<img src="./images/portal02-1.png" /><br/>
<img src="./images/portal02-2.png" /><br/><br/>
次に、Azure Search のサービス名と API Key を入力します。
<img src="./images/app03.png" width="70%"/><br/><br/>
<a href="https://portal.azure.com/">Azure Portal</a> で、Azure Search サービスを作成する手順を記載しておきます。<br/>
「＋リソースの作成」⇒「Web」⇒「Azure Search」を選択してください。 
<img src="./images/portal03-1.png" /><br/><br/>
「URL」にグローバルでぶつからないサービス名を入れ、適切な「Resouce group」を選択するか、新規で作成し、「Region (場所)」には「西日本」を選択します。最後に、「Pricing tier」ですが、機能検証レベルであれば、「Free」か「Basic」を選択し、「作成」をクリックしてください。
<img src="./images/portal03-2.png" /><br/><br/>
数分で Azure Search サービスが作成されますので、作成された後、以下の画面からサービス名と API Key を取得してください。
<img src="./images/portal03-3.png" /><br/><br/>
設定の最後は、Storage Account 名、Storage Key、Blob コンテナー名の入力です。
<img src="./images/app04.png" width="70%"/><br/><br/>
<a href="https://portal.azure.com/">Azure Portal</a> で、Storage Account (ストレージ アカウント) を作成後、以下の画面から Storage Account 名と Storage key を取得してください。
<img src="./images/portal04-1.png" /><br/><br/>
Storage Account の Blob カテゴリーからコンテナーを以下の「＋コンテナー」ボタンから作成してください。ここでは、「ocrimages」という名称のコンテナーを作成しています。
<img src="./images/portal04-2.png" /><br/><br/>
一度、「設定の保存」ボタンをクリックし、設定内容を保存します。
<img src="./images/app05.png" width="70%"/><br/><br/>
WPF アプリケーションのメイン画面が表示されますので、「初期設定」メニューの「設定画面の表示」をクリックします。
<img src="./images/app06.png" /><br/><br/>
再度、設定画面が表示されますので、設定内容に間違いがないか確認した後、「検索インデックスの作成 ＆ 削除」をクリックします。これにより、アプリケーションが Azure Search Service REST API を使用して、ソースコードの「WpfAppCvSearch\WpfAppCvSearch\Resources」フォルダーにある「qcdocs.schema.json」スキーマファイルの定義に基づいたインデックス「qcdocs」を Azure Search サービス上に作成します。既に同名のインデックスが存在する場合、削除されますので、ご注意ください。 
<img src="./images/app07.png" width="70%"/><br/>
<img src="./images/app08.png" width="70%"/><br/><br/>
インデックスの作成が成功しますと、<a href="https://portal.azure.com/">Azure Portal</a> の該当の Azure Search サービスで、以下のようにインデックスが作成されたことが分かります。この時点では、データが投入されていない為、「DOCUMENT COUNT」の値は、０となります。
<img src="./images/portal08-1.png" /><br/><br/>

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
