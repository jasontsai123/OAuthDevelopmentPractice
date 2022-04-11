# OAuthDevelopmentPractice
20220326 OAuth 2.0 開發實戰 (線上授課) 授權流程（用戶端）回家作業
https://oauth-development-practice.azurewebsites.net/
- 實作使用者登入(LINE Login)
- 實作使用者訂閱通知功能(LINE Notify) (取得Access Token 並儲存)
- 實作使用者訂閱成功頁面(LINE Notify)
- 實作使用者取消訂閱功能(LINE Notify) (撤銷Access Token 才行)
- 實作後台發送推播訊息功能(可以發送訊息給所有訂閱的人) (LINE Notify)

### 主要功能
- Line[登入/登出](https://oauth-development-practice.azurewebsites.net/Login)
- Notify[訂閱](https://oauth-development-practice.azurewebsites.net/LineNotify)
- Notify取消[訂閱](https://oauth-development-practice.azurewebsites.net/LineNotify)
- [發送訊息](https://oauth-development-practice.azurewebsites.net/Admin/SendNotifyMessage)給訂閱者

*資料儲存用EFCore SQLite in-memory的模式

### 情境描述
![程式畫面](https://user-images.githubusercontent.com/31040621/162583467-b1e052d6-e08a-4033-b41c-450c184a1cd8.png)

主要是使用 LINE Notify 的訂閱推播服務，訂閱之前要先登入Line，以下為操作步驟。
1. 點[Line通知訂閱]，沒登入則跳到登入頁，點Line登入的按鈕，成功登入後會跳回原來的頁面也就是[Line通知訂閱]，登入按鈕也會變為登出。
2. 這時候會看到綠色訂閱按鈕，按下去後會連到Notify讓你設定要在哪個頻道連動(通常都選LINE Notify官方帳號，其他頻道要把LINE Notify官方帳號加入群組才會生效)，連動成功LINE Notify會發訊息，此時按鈕會變成紅色框的取消訂閱按鈕。
3. 可點[管理]裡面有列表可以查看是否有資料，資料是使用者訂閱的資料表，有新增一筆資料代表有訂閱成功，發送通知會發送給此資料表裡面的成員。
4. 點[送出訊息]輸入標題和內容，按SEND送出，會跳出 '訊息已送出!' 的訊息。
5. 再點回[Line通知訂閱]，按取消訂閱按鈕，LINE Notify會發出連動已解除的訊息。
6. 按登出，登出成功會導轉到首頁，登出按鈕會變為登入。

### 實作描述
LINE Notify 和 LINE Login 基本上沒有連動關係，可將兩者視為獨立平台，可獨立開發，這邊先從 Notify 開發，因為上課時已經有先用 Postman 實作過通知流程，以下為過程

#### LINE Notify 開發
```
Step 1 登入
https://webhook.site/login![2022-04-11_01h00_25](https://user-images.githubusercontent.com/31040621/162630827-aa80c13e-2170-4095-8eac-2daad72e46d4.png)


Step 2 取得授權
https://notify-bot.line.me/oauth/authorize?
response_type=code
&client_id=7u1Lu4cdIldcwOx9ueDBaJ
&state=123123
&scope=notify
&redirect_uri=https://webhook.site/890273bc-41ed-4e2a-850c-698b65b93018

Step3 取得授權碼
https://webhook.site/890273bc-41ed-4e2a-850c-698b65b93018?code=PmqgFHP9ipyrMKMOQk5tiL&state=123123

Step4 取得訪問Token
[POST] https://notify-bot.line.me/oauth/token
Request Key/Value
grant_type:authorization_code
code:PmqgFHP9ipyrMKMOQk5tiL
redirect_uri:https://webhook.site/890273bc-41ed-4e2a-850c-698b65b93018
client_id:< your client_id >
client_secret:< your client_secret >
Get Token
{
    "status": 200,
    "message": "access_token is issued",
    "access_token": "ZpG6Tcn1X44Kfxxy5duLT7fertSz3UdveyicFLnSvJn"
}

Step5 傳送訊息
[POST] https://notify-api.line.me/api/notify
Authorization: Bearer "ZpG6Tcn1X44Kfxxy5duLT7fertSz3UdveyicFLnSvJn"
{"status":200,"message":"ok"}
```
依據上述步驟可以發現這個功能做主要的目的就是取得 AccessToken ，拿到 AccessToken 訂閱、推播、取消訂閱功能都可完成。
> 這是初學時的想法，現在有概念覺得這段很像廢話😂

既然知道實作流程，把步驟往前推我覺得比較好切入開發。取得 AccessToken 之前要先取得授權碼Code  
(client_id、client_secret 建立 Notify 頻道就取得所以這裡只剩 code 要關注)
而 Code 是從`https://notify-bot.line.me/oauth/authorize`API導轉 Callback Url 把 Response 的資料帶過來，這裡多加了`response_mode = "form_post"`，把 Response 的資料 Post 到 CallbackUrl ，這樣參數就不會顯示在瀏覽器網址上，安全性更有保障。  

然後開一個 Post 方法的 MVC Action 來接Code參數，將這個 Action 路由註冊為 Callback Url e.g.`https://localhost:5001/LineNotify/Subscribe`(這邊 Callback Action 路由命名 Callback 可能比較明確一些)，這個Action就負責註冊的邏輯，例如取得Token然後寫進資料庫。  

POST `https://notify-bot.line.me/oauth/token` 取 Token，這裡有兩個重要的參數，一個是如同前面提到的授權碼Code，另一個很重要的是 redirect_uri ，這裡的 redirect 不會真的導轉，比較像是和 code 一樣要驗證的參數，就是前一個打`https://notify-bot.line.me/oauth/authorize` API 步驟授權的 Callback Url。  

再來最後一個步驟(實際上是流程的第一步驟)透過`https://notify-bot.line.me/oauth/authorize`取得授權，對於開發上來說這段程式主要目的就是看要導轉到哪個 Callback ， 這個 API 只有 GET 方法，所以要直接寫在前端連結也是可以，這裡我是寫在 Subscribe 的 GET 方法裡，從前端訂閱按鈕連結到 Subscribe Action ， 再自己導轉到`https://notify-bot.line.me/oauth/authorize`。 state 參數隨便給個值不會影響功能實現，這個參數存在目的是為了防止 CSRF 攻擊，應該用程式產生不固定的值當參數 e.g. Random。
之後的通播訊息和取消訂閱的功能都是 API 請求 Header 直接帶`Authorization Bearer <access_token>`就行。

總結 Notify 開發主要會用到以下 API
https://notify-bot.line.me/doc/en/
```
GET https://notify-bot.line.me/oauth/authorize
POST https://notify-bot.line.me/oauth/token
POST https://notify-api.line.me/api/notify
POST https://notify-api.line.me/api/revoke
```
官方文件小彩蛋  
![神奇的空格](https://user-images.githubusercontent.com/31040621/162588633-b8c94bbe-1b55-4a8c-abeb-fb70cf2e2bc2.png)  
複製網址請小心😒

簡化從訂閱到推播和取消的流程
`/oauth/authorize` --> `callback url` --> `/oauth/token` --> `/api/notify` --> `/api/revoke`

#### LINE Login 開發
這裡要驗證登入的頁面不會直接導到 LINE Login 網站，一律都先在 Login 頁面按LINE登入按鈕登入，未來要加其他登入選則會比較方便擴充。

程序上與 LINE Notify 大同小異，開發主要會用到以下 API
https://developers.line.biz/en/reference/line-login/
https://developers.line.biz/zh-hant/docs/line-login/integrate-line-login/
```
GET https://access.line.me/oauth2/v2.1/authorize
POST https://api.line.me/oauth2/v2.1/token
POST https://api.line.me/oauth2/v2.1/revoke
GET https://api.line.me/v2/profile
```

流程
`需要驗證的頁面` --> `/Login` --> `選擇登入方式` --> `/Line` --> `access.line.me/oauth2/v2.1/authorize` --> `callback url` --> `api.line.me/oauth2/v2.1/token` --> `api.line.me/v2/profile` --> `api.line.me/oauth2/v2.1/revoke`
> callback Action 參數最好加上 `error` 會比較好 Debug

有需要存取使用者 Email 需要另外申請許可  
![LINE申請Email](https://user-images.githubusercontent.com/31040621/162631077-b0699cb0-fbdf-4f60-9586-32ab7d729be7.jpg)

最後 Line Developer Channel 一定要確認是不是 Published 的狀態，不然會出現 `400 Bad Request`，真的很感謝其他同學提醒，不然不知道又要 Debug 多久XD
![Line Developer的設定要記得調整成published](https://user-images.githubusercontent.com/31040621/162630838-3ab74b0d-ddc0-40e7-84d0-4a8893df5ee3.png)

### 專案開發參考的文章
https://stackoverflow.com/questions/56319638/entityframeworkcore-sqlite-in-memory-db-tables-are-not-created
https://flurl.dev/
https://stackoverflow.com/questions/4015324/how-to-make-an-http-post-web-request
https://docs.microsoft.com/zh-tw/ef/core/testing/testing-without-the-database
https://ithelp.ithome.com.tw/articles/10192537
https://www.dotblogs.com.tw/wasichris/2021/11/10/231809
https://stackoverflow.com/questions/2201238/how-does-request-isauthenticated-work
https://dotblogs.com.tw/Null/2020/04/09/162252
