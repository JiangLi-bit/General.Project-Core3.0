# General.Project
.Net Core 3.0

swagge<br/>
Autofac 第三方依赖注入<br/>
JWT令牌认证<br/>
动态策略授权（通过角色获取对应权限及访问路径进行匹配）<br/>
实体类（Entity）：数据库实体对象<br/>
仓储模块（IRepository、Repository作为一个数据库管理员，直接操作数据库，实体模型）：BaseRepository（基类仓储） 继承实现了 接口IBaseRepository，这里放公共的方法<br/>
Service模块（IService、Service处理业务逻辑，可以直接使用ViewModel视图模型）：BaseService 调用 BaseRepository，同时继承 IBaseService <br/>
公共模块（Common）:一些通用方法及公用类<br/>
辅助模块（Framework）：一扩展类及方法
