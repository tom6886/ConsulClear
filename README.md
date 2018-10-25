# ConsulClear
移除consul过期服务工具  

## 开发原因
当在Spring Cloud应用中使用Consul来实现服务治理时，由于Consul不会自动将不可用的服务实例注销掉（deregister），
这使得在实际使用过程中，可能因为一些操作失误、环境变更等原因让Consul中存在一些无效实例信息，
而这些实例在Consul中会长期存在，并处于断开状态。它们虽然不会影响到正常的服务消费过程，
但是它们会干扰我们的监控，所以我们可以实现一个清理接口，在确认故障实例可以清理的时候进行调用来将这些无效信息清理掉。

## 解决办法
调用deregister接口  
用PUT请求Consul 的这个deregister接口，附上实例的id就可以成功注销掉实例了（注意是实例的id，不是服务名，即服务名+一段唯一字符串。
有ACL认证的Consul需要在Header上加token，否则会报permission denied）

示例：put方式访问http://XXX.XX.xx.xxx:8500/v1/agent/service/deregister/xxx-xxx-x-8099

但是这样只能单个删除服务，非常麻烦  
开发此工具就是为了批量移除consul里的服务

## 使用方式
### 客户端程序
【1】开发者模式：  
下载源码，使用VS打开Consul.Clear.Winform文件夹下的程序，运行即可  
【2】安装模式：  
install文件夹下有客户端安装包的压缩文件，下载解压后安装即可
### web端程序
【1】开发者模式：  
下载源码，打开Consul.Clear.Web文件夹下的程序，运行即可   
【2】部署模式：  
web端提供了两种部署方式，jar包部署以及docker部署，推荐使用docker方式部署  
解压install文件夹下的web端安装包，具体安装方式请自行百度
