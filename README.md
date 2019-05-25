## WifiManager ##

![image](https://github.com/tiancai4652/WifiManager.WPF/blob/master/1.png)

**主要使用的技术：**
SimpleWifi，MahaApp.Metro控件

### 一 网卡设置 ###

#### 1.获取所有网卡(NetWorkAdapter类) ####

方法A   
通过`API SELECT * FROM Win32_NetworkAdapterConfiguration`获取网卡列表
方法B  
调用`NetworkInterface.GetAllNetworkInterfaces()`返回本地计算机上的网络接口对象  
**方法B相比于方法A的区别在于方法B只能找出启用的网卡对象，而方法A可以获取全部的网卡列表，无论是启用还是禁用都可以获取。**由此我们可以通过对比这两个列表来判断一个网卡是启用状态还是禁用状态。
关于判断网卡启用还是禁用除了这个方法外并没找到更好地办法。


#### 2.检测是否为无线网卡 ####

如果电脑已经识别出了无线网卡，则会在注册表上注册，所有可以通过注册表检测是否为无线网卡

    SYSTEM\\CurrentControlSet\\Control\\Network\\{4D36E972-E325-11CE-BFC1-08002BE10318}\\" + mo["SettingID"] + "\\Connection"
MediaSubType==2为无线网卡

#### 3.设置网卡启用/禁用 ####
通过执行命令行进行对网卡的启用/禁用  

    string cmd = ($"wmic path win32_networkadapter where index={network["Index"]} call ");
    cmd += state ? "enable" : "disable";
    RunCMD(cmd, out string strOutput);

### Tips ###

- 当开始启用网卡的时候，如果立即获取所有wifi列表，是获取不全的，过个几十毫秒再去获取就能获取全了
- SimpleWifi有个连接/断开wifi的事件，很有用


**项目地址:https://github.com/tiancai4652/WifiManager.WPF**


