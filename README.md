# Computer Vision API (OCR) v2 と Azure Search を用いて、スキャンされた画像文書を検索＆プレビューできるようにしたサンプル WPF アプリケーション
アプリケーションの実行に当たっては、「WpfAppCvSearch」フォルダーのソリューションファイルを Visual Studio 2017 で開き、ビルド/リビルドしてください。ビルド時にエラーになる場合、一度、ソリューションのクリーンをしてから再度ビルドをしてください。ソースコードが不要な方には、バイナリも用意しておりますので、Installer.zip ファイルをダウンロードして解凍し、setup.exe を起動してインストールしてください。
<br/><br/>
## アプリケーション構成
<img src="./images/app00.png" /><br/>
<br/><br/>
## アプリケーションの実行
### 初期設定
アプリケーションを初めて実行する場合、以下の設定画面が最初に開きます。ここでは、<a href="https://westus.dev.cognitive.microsoft.com/docs/services/5adf991815e1060e6355ad44/operations/56f91f2e778daf14a499e1fc">Cognitive Services - Computer Vision API (OCR) V2</a> 用の設定情報、<a href="https://docs.microsoft.com/ja-jp/azure/search/search-what-is-azure-search">Azure Search</a> 用の設定情報、<a href="https://docs.microsoft.com/ja-jp/azure/storage/common/storage-introduction">Azure Storage</a> 用の設定情報を入力する必要があります。<br/>
<img src="./images/app01.png" width="70%"/><br/><br/>
最初に、Computer Vision API (OCR) V2 用のエンドポイントと API Key を入力します。<br/>
<img src="./images/app02.png" width="70%"/><br/><br/>
<a href="https://portal.azure.com/">Azure Portal</a> で、AI + Machine Learning カテゴリーにある Cognitive Services - Computer Vision を作成すると、以下の２つの画面からエンドポイントと API Key を取得できます。アプリケーションの設定画面には、ここで得られたエンドポイントの文字列に、"vision/v2.0/ocr" を加えて入力してください。<br/>
<img src="./images/portal02-1.png" /><br/>
<img src="./images/portal02-2.png" /><br/><br/>
次に、Azure Search のサービス名と API Key を入力します。<br/>
<img src="./images/app03.png" width="70%"/><br/><br/>
<a href="https://portal.azure.com/">Azure Portal</a> で、Azure Search サービスを作成する手順を記載しておきます。<br/>
「＋リソースの作成」⇒「Web」⇒「Azure Search」を選択してください。 <br/>
<img src="./images/portal03-1.png" /><br/><br/>
「URL」にグローバルでぶつからないサービス名を入れ、適切な「Resouce group」を選択するか、新規で作成し、「Region (場所)」には「西日本」を選択します。最後に、「Pricing tier」ですが、機能検証レベルであれば、「Free」か「Basic」を選択し、「作成」をクリックしてください。<br/>
<img src="./images/portal03-2.png" /><br/><br/>
数分で Azure Search サービスが作成されますので、作成された後、以下の画面からサービス名と API Key を取得してください。<br/>
<img src="./images/portal03-3.png" /><br/><br/>
設定の最後は、Storage Account 名、Storage Key、Blob コンテナー名の入力です。<br/>
<img src="./images/app04.png" width="70%"/><br/><br/>
<a href="https://portal.azure.com/">Azure Portal</a> で、Storage Account (ストレージ アカウント) を作成後、以下の画面から Storage Account 名と Storage key を取得してください。<br/>
<img src="./images/portal04-1.png" /><br/><br/>
Storage Account の Blob カテゴリーからコンテナーを以下の「＋コンテナー」ボタンから作成してください。ここでは、「ocrimages」という名称のコンテナーを作成しています。<br/>
<img src="./images/portal04-2.png" /><br/><br/>
一度、「設定の保存」ボタンをクリックし、設定内容を保存します。<br/>
<img src="./images/app05.png" width="70%"/><br/><br/>
WPF アプリケーションのメイン画面が表示されますので、「初期設定」メニューの「設定画面の表示」をクリックします。<br/>
<img src="./images/app06.png" /><br/><br/>
再度、設定画面が表示されますので、設定内容に間違いがないか確認した後、「検索インデックスの作成 ＆ 削除」をクリックします。これにより、アプリケーションが Azure Search Service REST API を使用して、ソースコードの「WpfAppCvSearch\WpfAppCvSearch\Resources」フォルダーにある「qcdocs.schema.json」スキーマファイルの定義に基づいたインデックス「qcdocs」を Azure Search サービス上に作成します。既に同名のインデックスが存在する場合、削除されますので、ご注意ください。<br/>
<img src="./images/app07.png" width="70%"/><br/>
<img src="./images/app08.png" width="70%"/><br/><br/>
インデックスの作成が成功しますと、<a href="https://portal.azure.com/">Azure Portal</a> の該当の Azure Search サービスで、以下のようにインデックスが作成されたことが分かります。この時点では、データが投入されていない為、「DOCUMENT COUNT」の値は、０となります。<br/>
<img src="./images/portal08-1.png" /><br/><br/>
### アプリケーションの利用
これで、初期設定はすべて完了しましたので、アプリケーションの利用を開始します。<br/>
「画像ファイルの選択」をクリックします。<br/>
<img src="./images/app11.png" /><br/><br/>
スキャンされた画像文書を選択します。複数選択可能です。<br/>
<img src="./images/app12.png" width="70%"/><br/><br/>
リストボックスに読み込まれた画像ファイル名をクリックすると、画像のイメージがプレビューされます。<br/>
<img src="./images/app13.png" /><br/><br/>
「画像分析プレビュー」をクリックすると、クリックされている画像ファイル名に対して OCR 機能のみが呼び出され、TEXT もしくは JSON 形式で、OCR で分析された結果が表示されます。これは、OCR の精度を確認する為だけの機能です。<br/>
<img src="./images/app14.png" /><br/>
<img src="./images/app15.png" /><br/><br/>
「タグ登録」メニューから「タグ登録画面の表示」をクリックすると、OCR で読み取られたテキストに対して、正規表現を使ってマッチさせることの出来るカスタムのタグ付与ルールを登録することが可能です。このタグ付与ルールは、設定画面で設定した Storage Account 名と Storage Key を使って、Azure Table Storage に登録されます。タグ付与のルールが登録されていると、検索インデックスへの登録時に正規表現を適用してマッチした場合に自動的にタグが付与されます。<br/>
<img src="./images/app16.png" /><br/>
<img src="./images/app17.png" width="70%"/><br/><br/>
メイン画面の「画像ファイル分析 ＆ 検索インデックスへの登録」ボタンを押します。<br/>
<img src="./images/app18.png" /><br/><br/>
「検索インデックス登録 - 属性入力」画面が表示されますので、プロジェクト名と場所を選択してください。これは、デモ的な属性入力ですので、プロジェクト名と場所の情報は、WPF の XAML 上にリスト定義しています。「登録の実施」ボタンをクリックしますと、リストボックスに読み込まれている全ての画像ファイルに対して、OCR を実行し、Blob Storage へのアップロード、タグ付与ルールの適用、Azure Search 「qcdocs」インデックスへのデータ登録が実施されます。<br/>
<img src="./images/app19.png" width="70%"/><br/>
<img src="./images/app20.png" /><br/><br/>
検索インデックスにデータを登録した後、以下の「検索」メニューの「検索画面の表示」をクリックします。<br/>
<img src="./images/app21.png" /><br/><br/>
検索したい文字列をスペースで繋いで、検索を実行します。この例では、先のタグ付与ルールで利用した文字列を使って検索しています。検索結果は、検索スコアリングの高い順に並びます。つまり、上の方がより高い精度での検索結果になります。検索結果をクリックすると、画像がプレビューされます。この時、検索結果の blobpath を基に、アップロードした先の Azure Blob Storage から画像ファイルをダウンロードして表示しています。<br/>
<img src="./images/app22.png" /><br/><br/>
検索インデックス スキーマの項目として、タグの配列を格納する項目を用意していますので、自動付与ルールで設定しているタグで検索すると、一番上位に (＝一番高い検索スコアリングに) なっています。<br/>
<img src="./images/app23.png" /><br/>
