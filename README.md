# zero_unity_demo

本Demo用[zero](https://github.com/9b9387/zero)做网络服务，Unity客户端实现与zero配套的网络功能，数据格式使用JSON。

Demo的需求
- 新玩家加入
- 在线玩家同步
- 玩家移动位置同步
- 玩家离开

![](https://github.com/9b9387/zero_unity_demo/blob/master/Document/demo.gif?raw=true)

运行Demo：

服务器
```go
cd Server
go get -u github.com/9b9387/zero
go run app.go
```

客户端
Demo开发环境使用Unity版本2018.3.2f1。其他版本没有试过。
用Unity打开UnityDemo文件夹，打开SampleScene运行即可。
