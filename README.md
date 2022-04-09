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
![2022-04-10_00h47_15](https://user-images.githubusercontent.com/31040621/162583467-b1e052d6-e08a-4033-b41c-450c184a1cd8.png)

主要是使用 LINE Notify 的訂閱推播服務，訂閱之前要先登入Line，以下為操作步驟。
1. 點[Line通知訂閱]，沒登入則跳到登入頁，點Line登入的按鈕，成功登入後會跳回原來的頁面也就是[Line通知訂閱]，登入按鈕也會變為登出。
2. 這時候會看到綠色訂閱按鈕，按下去後會連到Notify讓你設定要在哪個頻道連動(通常都選LINE Notify官方帳號，其他頻道要把LINE Notify官方帳號加入群組才會生效)，連動成功LINE Notify會發訊息，此時按鈕會變成紅色框的取消訂閱按鈕。
3. 可點[管理]裡面有列表可以查看是否有資料，資料是使用者訂閱的資料表，有新增一筆資料代表有訂閱成功，發送通知會發送給此資料表裡面的成員。
4. 點[送出訊息]輸入標題和內容，按SEND送出，會跳出 '訊息已送出!' 的訊息。
5. 再點回[Line通知訂閱]，按取消訂閱按鈕，LINE Notify會發出連動已解除的訊息。
6. 按登出，登出成功會導轉到首頁，登出按鈕會變為登入。

