# 見積管理システム

Winforms(VB.NET)のサンプルアプリケーションです.

見積の作成・印刷を行うアプリです.

業務システム構築の際のアプリケーション設計を考えるために作成されました.

Visual Studio 2026 (Community) で開発・ビルドすることを想定しています.

## 利用ミドルウェア

このアプリケーションは以下のミドルウェアを利用しています.

 1. ADO.NET (`System.Data.SQLite.Core`)
 1. Microsoft Report Viewer (`Microsoft.ReportingServices.ReportViewerControl.Winforms`)
 1. MSTest

ターゲットフレームワークは **.NET Framework 4.8** です（Developer Pack のインストールが必要です）。

インストーラ (`Setup/Setup.vdproj`) はソリューションから除外しています。再ビルドする場合は Visual Studio Marketplace の **Microsoft Visual Studio Installer Projects** 拡張を入れ、ソリューションにプロジェクトを追加してください。

## 機能

見積書を作成するだけのアプリケーションです.

5種類の台帳(いわゆるマスタ)と見積の登録/管理機能があります.

#### マスタ

 * 顧客台帳
 * 従業員台帳
 * 支払条件の設定
 * 消費税の設定
 * 自社情報の設定

#### トランザクション

 * 見積の作成
 * 見積の管理

#### 他

 * バックアップの作成
 * バックアップのインポート
 * ウィンドウ制御

## アプリケーション設計/構成

このアプリケーションは7つのプロジェクトで構成されています（Setup は別途）。

```
├─ADOWrapper
│  ├─Interface
│  └─SQLite3
├─Application
│  ├─Component
│  ├─Form
│  ├─Report
├─Domain
│  ├─DomainObject
│  └─Repository
├─Infrastructure
│  ├─DDL
│  └─RepositoryImpl
├─TestADOWrapper
├─TestDomain
├─TestInfrastructure
```

Testから始まるプロジェクトは各プロジェクトのテストプロジェクトです.

各プロジェクトの依存関係は以下の通りです.(Testプロジェクトを除く)

```
---------------        --------------- 
| Domain      |        | ADOWrapper  |
---------------        ---------------
   ^        ^            ^
   |        |            |
   |     ------------------
   |     | Infrastructure |
   |     ------------------
   |        ^
   |        |
---------------
| Application |
---------------
```

------

ADOWrapperはADO.NETの簡易なラッパーです.

このソフトウェア全体を通して, DBへのアクセスにはこのADOWrapperを用いています.

------

Infrastructureは永続化のための機能を提供します.
これは, DomainプロジェクトのRepositoryインターフェースに対する実装です.
RDBMSを差し替える場合にはおそらくInfrastructure全体を差し替える事になります.


(ADOWrapperもInterfaceと実装が分かれているため, 基本的にはRDBMSの差異を吸収することができます.
 しかし, たいていの業務アプリケーションはRDBMS実装依存のSQLを使わざるおえないでしょう.
 そうなるとSQLを記述するInfrastractureプロジェクトの差し替えが必要です.)

------

Domainは業務的に意味のある塊をクラスにしたものの集まりです.

入力の妥当性確認や計算(見積なら合計金額や税率計算)などもこのプロジェクトのクラスで行っています.

(規模が小さいのでO/Rマッパーを使えば楽そうに見えたりもしますが, 規模が大きくなると・・・)

------

Applicationは普通のWindows Form Application(いわゆるWinformsアプリ)です.

画面とのデータのやり取りは極力データバインディングを利用しています.

Domainプロジェクトのデータ構造をそのままバインディングしている箇所もあれば,
間にPresentation(Data Transfer Object)というクラスを中継してバインディングしている所もあります.

単純なマスタの場合など, 直接Domainプロジェクトのクラスをバインディングしている箇所が多いですが,
検索して表示する一覧などは後からドメインオブジェクトを跨いだ項目表示を要求されることが多いという経験上, Presenterを挟むようにしています.
```
      -----------------
      | Event Handler |
      -----------------
        | Control  |
        |          |
-------------   ------------------
| Presenter |---| Domain Object  |
-------------   ------------------
        |          |
        | Data     |
        | Binding  |
        |          |
      ------------------
      | User Interface |
      ------------------
```

## ビルド

Visual Studio 2026 で `SalesManagement.sln` を開き、NuGet 復元後にビルドしてください。

コマンドライン例:

```bat
msbuild SalesManagement.sln /t:Restore,Build /p:Configuration=Debug
```

## デモデータ

プロジェクトルートのDemoDataフォルダにあるdemo.dbをインポートしてください.

## LICENSE

MIT Licenseです.

無保証ですが商用非商用, 改変/無改変問わず, ご自由にお使いください.  
(使い道は思いつきませんが)
