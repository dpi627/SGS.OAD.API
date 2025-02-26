![](https://img.shields.io/badge/SGS-OAD-orange) 
![](https://img.shields.io/badge/proj-SGS.OAD.API-purple) 
![](https://img.shields.io/badge/-9-512BD4?logo=dotnet)
![](https://img.shields.io/badge/-OpenAPI-555?logo=openapiinitiative)
![](https://img.shields.io/badge/-Scalar-0F0F0F?logo=scalar)
![](https://img.shields.io/badge/-Grok-555?logo=x)
![](https://img.shields.io/badge/-OpenAI-412991?logo=openai)
![](https://img.shields.io/badge/GitHub_Copilot-555?logo=githubcopilot)
![](https://img.shields.io/badge/-draw.io-555?logo=diagrams.net)
![](https://img.shields.io/badge/-Git-666?logo=git)
![](https://img.shields.io/badge/-GitHub-181717?logo=github)
![](https://img.shields.io/badge/-Gitea-666?logo=gitea)

# SGS.OAD.API

- ASP.NET Core 9 WebAPI，遵循 RESTful
- 提供 AD 驗證與獲取 HR 開放資料端點
- 提供 API 健康狀態檢查端點

# UAT

https://twtpeoad001.sgs.net/api/scalar/


> 💡使用 [Scalar](https://scalar.com/) 作為測試介面

# Architecture

```js
📁SGS.OAD.API
  📁Controller  //--- api endpoints
  📁Models      //--- data structures
  📄appsettings.json
  📄appsettings.Development.json  //--- develop settings
  📄appsettings.Production.json   //--- production settings
📄README.md     //--- this doc
📄nuget.config  //--- internal package source
```

- 採用 Controller，非 Minium API
- `nuget.config` 定義了內部 NuGet pacakage source
- ⚠️部署時需確認(或修改) `appsettings.Production.json` 之 Base URL

# Endpoints

## AD

```
/ad/vaild
```

- 傳入 `json`，包含 AD 帳號、密碼與域名(預設 `APAC`)
- 因包含敏感資料，故設計為 `POST`，避免密碼直接暴露於網誌
- 取得帳號驗證結果 `true/false`，詳細規格參考線上文件

```
/ad/info
```

- 帶入資料與前者相同，使用路由名稱進行區分
- 因包含敏感資料，故設計為 `POST`
- 取得部分開放之 AD 資料

## Emp

```
/emp/{adAccount}
```

```
/emp/{idType}/{id}
```


## Health

```
/health
```


# Dependency

- SGS.OAD.AdAuth
- SGS.OAD.HrInfo