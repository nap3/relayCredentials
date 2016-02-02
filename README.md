# relayCredentials #########################
アプリケーションのプロキシ認証情報を中継するモジュール。

## Description
認証プロキシサーバーの認証情報を保存できず何度も認証情報を要求してくるアプリケーションや
そもそも認証情報を入力することができないアプリケーションが存在します。
それらのアプリケーションに対して認証情報の中継を行います。
認証情報の中継が可能なアプリケーションは.Net Frameworkで作成されたアプリケーションです。



## Features
* 設定ファイルで指定したプロキシサーバーに認証情報を渡す。
* 設定ファイルに指定したBypassListに対してはプロキシサーバーを経由しない。

##Requirement

* .Net Framework 2.0 or higher version     
* Visual C# 2010(If build this project.)


## Usage     
1. 認証情報を中継したいアプリケーションのapp.config（Excelの場合、Excel.config）に以下のコードを追加します。  
（app.configが存在しない場合は新しいファイルを作成します。）
1. relayCredentialsSetting.xmlに中継するプロキシサーバー名と認証情報を書き込みます。
1. 認証情報を中継したいアプリケーションと同じ場所にrelayCredentialsSetting.dll、relayCredentialsSetting.xmlを配置します。


app.configの修正箇所

```xml
  <system.net>
    <defaultProxy>
      <module type="relayCredentials.AuthProxyModule, relayCredentials"/>
    </defaultProxy>
  </system.net>
```

[Excelの場合のサンプルファイルを置いています。](https://github.com/nap3/relayCredentials/blob/master/relayCredentials/Excel.exe.config)


## Licence

[MIT](https://github.com/nap3/relayCredentials/blob/master/LICENSE)


## Author

[nap3](https://github.com/nap3)


---------------------------
## Thanks

Rido氏の [How to connect to TFS through authenticated Web Proxy](http://blogs.msdn.com/b/rido/archive/2010/05/06/how-to-connect-to-tfs-through-authenticated-web-proxy.aspx) という記事に書かれているRido.AuthProxyからヒントをもらいました。     
このブログのおかげで、defaultProxy-module 要素の使い方を理解することができました。     
ありがとうございました。

## Reference

* [How to connect to TFS through authenticated Web Proxy](http://blogs.msdn.com/b/rido/archive/2010/05/06/how-to-connect-to-tfs-through-authenticated-web-proxy.aspx)
* [<module> 要素 (ネットワーク設定)](https://msdn.microsoft.com/ja-jp/library/6w93fssz(v=vs.110).aspx)

