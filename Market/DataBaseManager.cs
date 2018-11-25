using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OracleClient;
using System.IO;
using System.Data;

namespace Market
{
    /// <summary> 数据库管理器
    /// </summary>
    class DataBaseManager
    {
        /// <summary> 数据库用户名
        /// </summary>
        private String User;
        /// <summary> 数据库密码
        /// </summary>
        private String Password;
        /// <summary> 登录验证成功标志
        /// </summary>
        public Boolean Success = false;
        /// <summary> 用于获取登录参数字符串
        /// </summary>
        /// <returns>返回 登录参数字符串</returns>
        private String GetConnectStr(String _User,String _Password)
        {
            return "User ID=" + _User + ";" + //用户名
                   "Password=" + _Password + //密码
                   ";Data Source=(DESCRIPTION = (ADDRESS_LIST= (ADDRESS = (PROTOCOL = TCP)(HOST = 127.0.0.1)(PORT = 1521))) (CONNECT_DATA = (SERVICE_NAME = xe)))";//默认localhost，端口1521
        }
        /// <summary> 初始化数据库管理器，测试连接
        /// </summary>
        /// <param name="_User">数据库账户名</param>
        /// <param name="_Password">数据库密码</param>
        public DataBaseManager(String _User,String _Password)
        {
            DataBaseLogin(_User, _Password);//验证登录数据库
        }
        /// <summary> 初始化数据库管理器，访问配置文件实现自动验证登录数据库
        /// </summary>
        public DataBaseManager()
        {
            if (File.Exists(Environment.CurrentDirectory + "/Config.inf"))//根目录存在配置文件
            {
                String[] Config_Lines = File.ReadAllLines(Environment.CurrentDirectory + "/Config.inf");//读取配置文件中所有行
                if (Config_Lines.Length >= 2)//配置文件中至少了保存用户名与密码
                {
                    DataBaseLogin(Config_Lines[0], Config_Lines[1]);//验证登录数据库
                }
            }
        }
        /// <summary> 数据库登录验证，验证成功则设置类中的数据库信息
        /// </summary>
        /// <param name="_User">数据库账户名</param>
        /// <param name="_Password">数据库密码</param>
        public void DataBaseLogin(String _User, String _Password)
        {
            String Connect_Str = GetConnectStr(_User, _Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                User = _User;//保存用户名
                Password = _Password;//保存密码
                Success = true;//标记数据库登录成功
            }
            catch (Exception)
            {//验证失败
                Success = false;
                User = "";
                Password = "";//清空保存的登录信息
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 创建员工表并插入默认超级管理员
        /// </summary>
        /// <returns>返回 true：成功 false：失败</returns>
        private Boolean CreateStaffTbl()
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand NewStafftbl = new OracleCommand(@"create table MarketTerminal_Staff
                                                                (
                                                                    StaffNo VARCHAR(5) PRIMARY KEY,
                                                                    StaffName VARCHAR(100) NOT NULL,
                                                                    SuperUser INT DEFAULT 0,
                                                                    Password VARCHAR(20)
                                                                )");//员工表
                NewStafftbl.Connection = Connect;//指定连接
                NewStafftbl.ExecuteNonQuery();//执行建表
                OracleCommand DefaultAdminInsert = new OracleCommand(@"insert into MarketTerminal_Staff
                                                                        values ('admin','DefaultSuperUser',1,'123')");//默认初始管理员
                DefaultAdminInsert.Connection = Connect;//指定连接
                DefaultAdminInsert.ExecuteNonQuery();//执行插入
                return true;//创建成功、插入成功
            }
            catch (Exception)
            {
                return false;//创建员工表失败则返回false
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 创建商品表
        /// </summary>
        /// <returns>返回 true：成功 false：失败</returns>
        private Boolean CreateGoodsTbl()
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand NewGoodstbl = new OracleCommand(@"create table MarketTerminal_Goods
                                                                (
                                                                    GoodsNo VARCHAR(20) PRIMARY KEY,
                                                                    GoodsName VARCHAR(100) NOT NULL,
                                                                    In_Price NUMBER NOT NULL,
                                                                    Out_Price NUMBER NOT NULL,
                                                                    MarketLeft INT NOT NULL,
                                                                    TrunkLeft INT NOT NULL,
                                                                    Brand VARCHAR(30) NOT NULL,
                                                                    Unit VARCHAR(8) NOT NULL
                                                                )");//商品表建表语句
                NewGoodstbl.Connection = Connect;//指定连接
                NewGoodstbl.ExecuteNonQuery();//执行建表
                return true;//创建成功
            }
            catch (Exception)
            {
                return false;//创建商品表失败则返回false
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 创建账目表
        /// </summary>
        /// <returns>返回 true：成功 false：失败</returns>
        private Boolean CreateAccountTbl()
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand NewAccounttbl = new OracleCommand(@"create table MarketTerminal_Account
                                                                    (
                                                                        AccountNo VARCHAR(20) PRIMARY KEY,
                                                                        GoodsNo VARCHAR(20) NOT NULL,
                                                                        GoodsName VARCHAR(100) NOT NULL,
                                                                        In_Price NUMBER NOT NULL,
                                                                        Out_Price NUMBER NOT NULL,
                                                                        OutNum INT NOT NULL,
                                                                        Unit VARCHAR(8) NOT NULL,
                                                                        Price NUMBER NOT NULL
                                                                    )");//账目表建表语句
                NewAccounttbl.Connection = Connect;//指定连接
                NewAccounttbl.ExecuteNonQuery();//执行建表
                return true;//创建成功
            }
            catch (Exception)
            {
                return false;//创建失败则返回false
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 创建商品表上针对于账目表的触发器
        /// </summary>
        /// <returns>返回 true：成功 false：失败</returns>
        private Boolean CreateTrigger()
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand NewTrigger = new OracleCommand(@"create or replace trigger T_UpAcc after update
                                                                on MarketTerminal_Goods
                                                                for each row
                                                                begin
                                                                 IF :new.MarketLeft<:old.MarketLeft THEN
                                                                    insert into MARKETTERMINAL_ACCOUNT
                                                                    values(to_char(systimestamp,'YYYYMMDDHH24MISSFF'),:new.GOODSNO,:new.GOODSName,:new.IN_PRICE,:new.OUT_PRICE,:old.marketleft-:new.marketleft,:new.unit,(:old.marketleft-:new.marketleft)*(:new.OUT_PRICE-:new.IN_PRICE));
                                                                 END IF;
                                                                end;");//创建触发器语句
                NewTrigger.Connection = Connect;//指定连接
                NewTrigger.ExecuteNonQuery();//执行创建
                return true;//创建成功
            }
            catch (Exception)
            {
                return false;//创建失败则返回false
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 创建引用游标与各个存储过程
        /// </summary>
        /// <returns></returns>
        private Boolean CreateCurandProc()
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand NewCur = new OracleCommand(@"create or replace package
                                                            PK_RefCur as
                                                            type p_cursor is ref cursor;
                                                            end PK_RefCur;");//创建引用游标
                NewCur.Connection = Connect;//指定连接
                NewCur.ExecuteNonQuery();//执行创建
                OracleCommand NewQGoodsList = new OracleCommand(@"create or replace procedure proc_GetGoodsList
                                                            (p_cur out PK_RefCur.p_cursor)
                                                            is
                                                            begin
	                                                            open p_cur for select * from MarketTerminal_Goods;
                                                            end;");//创建获取商品列表存储过程
                NewQGoodsList.Connection = Connect;//指定连接
                NewQGoodsList.ExecuteNonQuery();//执行创建
                OracleCommand NewQStaffList = new OracleCommand(@"create or replace procedure proc_GetStaffList
                                                            (p_cur out PK_RefCur.p_cursor)
                                                            is
                                                            begin
	                                                            open p_cur for select * from MarketTerminal_Staff;
                                                            end;");//创建获取员工列表存储过程
                NewQStaffList.Connection = Connect;//指定连接
                NewQStaffList.ExecuteNonQuery();//执行创建
                OracleCommand NewChkUsr = new OracleCommand(@"create or replace procedure CheckStaff
                                                            (v_USER in varchar,
                                                            v_PWD in varchar,
                                                            v_Ret out integer)
                                                            is
                                                            begin
	                                                            select count(*) into v_Ret
	                                                            from MarketTerminal_Staff
	                                                            where StaffNo=v_USER and Password=v_PWD;
                                                            end;");//创建员工登录存储过程
                NewChkUsr.Connection = Connect;//指定连接
                NewChkUsr.ExecuteNonQuery();//执行创建
                OracleCommand NewChkSU = new OracleCommand(@"create or replace procedure CheckSU
                                                            (v_USER in varchar,
                                                            v_Ret out integer)
                                                            is
                                                            begin
	                                                            select SuperUser into v_Ret
	                                                            from MarketTerminal_Staff
	                                                            where StaffNo=v_USER;
                                                            end;");//创建检查用户类型存储过程
                NewChkSU.Connection = Connect;//指定连接
                NewChkSU.ExecuteNonQuery();//执行创建
                OracleCommand UPUserName = new OracleCommand(@"create or replace procedure UPUserName
                                                            (v_USERid in varchar,
                                                            v_newUSER in varchar)
                                                            is
                                                            begin
	                                                            update MarketTerminal_Staff
	                                                            set StaffName=v_newUSER
	                                                            where StaffNo=v_USERid;
                                                            end;");//创建更新用户名存储过程
                UPUserName.Connection = Connect;//指定连接
                UPUserName.ExecuteNonQuery();//执行创建
                OracleCommand UPUserPWD = new OracleCommand(@"create or replace procedure UPUserPwd
                                                            (v_USERid in varchar,
                                                            v_newPWD in varchar)
                                                            is
                                                            begin
	                                                            update MarketTerminal_Staff
	                                                            set PASSWORD=v_newPWD
	                                                            where StaffNo=v_USERid;
                                                            end;");//创建更新用户密码存储过程
                UPUserPWD.Connection = Connect;//指定连接
                UPUserPWD.ExecuteNonQuery();//执行创建
                OracleCommand UPSUsta = new OracleCommand(@"create or replace procedure SetSUsta
                                                        (v_USERid in varchar,
                                                        v_SUstatus in integer)
                                                        is
                                                        begin
	                                                        update MarketTerminal_Staff
	                                                        set SuperUser=v_SUstatus
	                                                        where StaffNo=v_USERid;
                                                        end;");//创建更新SU状态存储过程
                UPSUsta.Connection = Connect;//指定连接
                UPSUsta.ExecuteNonQuery();//执行创建
                OracleCommand DeleteUser = new OracleCommand(@"create or replace procedure DeleteUser
                                                                (v_USERid in varchar)
                                                                is
                                                                begin
	                                                                delete from MarketTerminal_Staff
	                                                                where StaffNo=v_USERid;
                                                                end;");//创建删除员工存储过程
                DeleteUser.Connection = Connect;//指定连接
                DeleteUser.ExecuteNonQuery();//执行创建
                OracleCommand GetUser = new OracleCommand(@"create or replace procedure GetStaff
                                                            (v_USERid in varchar,
                                                            p_cur out PK_RefCur.p_cursor)
                                                            is
                                                            begin
	                                                            open p_cur for select * from MarketTerminal_Staff where StaffNo=v_USERid;
                                                            end;");//创建获取员工信息存储过程
                GetUser.Connection = Connect;//指定连接
                GetUser.ExecuteNonQuery();//执行创建
                OracleCommand NewUser = new OracleCommand(@"create or replace procedure NewStaff
                                                            (v_USERid in varchar,
                                                            v_UserName in varchar,
                                                            v_SU in integer,
                                                            v_UserPWD in varchar)
                                                            is
                                                            begin
	                                                            insert into MarketTerminal_Staff
	                                                            values(v_USERid,v_UserName,v_SU,v_UserPWD);
                                                            end;");//创建新建员工存储过程
                NewUser.Connection = Connect;//指定连接
                NewUser.ExecuteNonQuery();//执行创建
                OracleCommand DeleteGoods = new OracleCommand(@"create or replace procedure DeleteGoods
                                                                (v_Goodsid in varchar)
                                                                is
                                                                begin
	                                                                delete from MarketTerminal_Goods
	                                                                where GoodsNo=v_Goodsid;
                                                                end;");//创建删除商品存储过程
                DeleteGoods.Connection = Connect;//指定连接
                DeleteGoods.ExecuteNonQuery();//执行创建
                OracleCommand NewChkGoods = new OracleCommand(@"create or replace procedure CheckGoods
                                                                (v_Goodsid in varchar,
                                                                v_Ret out integer)
                                                                is
                                                                begin
                                                                    select count(*) into v_Ret
                                                                    from MarketTerminal_Goods
                                                                    where GoodsNo=v_Goodsid;
                                                                end;");//创建检查商品存储过程
                NewChkGoods.Connection = Connect;//指定连接
                NewChkGoods.ExecuteNonQuery();//执行创建
                OracleCommand UpGoodsNum = new OracleCommand(@"create or replace procedure UpGoodsNum
                                                                (v_Goodsid in varchar,
                                                                v_Num in integer)
                                                                is
                                                                begin
                                                                    update MarketTerminal_Goods
                                                                     set TrunkLeft = TrunkLeft + v_Num
                                                                     where GoodsNo = v_Goodsid;
                                                                end;");//创建更新商品数目存储过程
                UpGoodsNum.Connection = Connect;//指定连接
                UpGoodsNum.ExecuteNonQuery();//执行创建
                OracleCommand UpGoodsNowNum = new OracleCommand(@"create or replace procedure UpGoodsNowNum
                                                                (v_Goodsid in varchar,
                                                                v_Num in integer)
                                                                is
                                                                begin
                                                                    update MarketTerminal_Goods
                                                                     set MARKETLEFT = MARKETLEFT + v_Num
                                                                     where GoodsNo = v_Goodsid;
                                                                end;");//创建更新商品货架数目存储过程
                UpGoodsNowNum.Connection = Connect;//指定连接
                UpGoodsNowNum.ExecuteNonQuery();//执行创建
                OracleCommand NewGoods = new OracleCommand(@"create or replace procedure NewGoods
                                                            (v_Goodsid in varchar,
                                                            v_GOODSNAME in varchar,
                                                            v_IN_PRICE in number,
                                                            v_OUT_PRICE in number,
                                                            v_MARKETLEFT in int,
                                                            v_TRUNKLEFT in int,
                                                            v_BRAND in varchar,
                                                            v_UNIT in varchar)
                                                            is
                                                            begin
                                                                insert into MarketTerminal_Goods
                                                                values (v_Goodsid,v_GOODSNAME,v_IN_PRICE,v_OUT_PRICE,v_MARKETLEFT,v_TRUNKLEFT,v_BRAND,v_UNIT);
                                                            end;");//创建更新创建商品存储过程
                NewGoods.Connection = Connect;//指定连接
                NewGoods.ExecuteNonQuery();//执行创建
                OracleCommand GetGoods = new OracleCommand(@"create or replace procedure GetGoods
                                                            (v_Goodsid in varchar,
                                                            p_cur out PK_RefCur.p_cursor)
                                                            is
                                                            begin
                                                                open p_cur for select * from MarketTerminal_Goods
                                                                where GoodsNo=v_Goodsid;
                                                            end;");//创建获取商品信息存储过程
                GetGoods.Connection = Connect;//指定连接
                GetGoods.ExecuteNonQuery();//执行创建
                OracleCommand UPGoods = new OracleCommand(@"create or replace procedure UPGoods
                                                            (v_Goodsid in varchar,
                                                            v_GOODSNAME in varchar,
                                                            v_IN_PRICE in number,
                                                            v_OUT_PRICE in number,
                                                            v_MARKETLEFT in int,
                                                            v_TRUNKLEFT in int,
                                                            v_BRAND in varchar,
                                                            v_UNIT in varchar)
                                                            is
                                                            begin
                                                                update MarketTerminal_Goods
                                                                    set GOODSNAME=v_GOODSNAME,
                                                                        IN_PRICE=v_IN_PRICE,
                                                                        OUT_PRICE=v_OUT_PRICE,
                                                                        MARKETLEFT=v_MARKETLEFT,
                                                                        TRUNKLEFT=v_TRUNKLEFT,
                                                                        BRAND=v_BRAND,
                                                                        UNIT=v_UNIT
                                                                        where GOODSNO=v_Goodsid;
                                                            end;");//创建更新商品信息存储过程
                UPGoods.Connection = Connect;//指定连接
                UPGoods.ExecuteNonQuery();//执行创建
                OracleCommand NewQAccfList = new OracleCommand(@"create or replace procedure proc_GetAccountList
                                                            (p_cur out PK_RefCur.p_cursor)
                                                            is
                                                            begin
	                                                            open p_cur for select * from MarketTerminal_Account;
                                                            end;");//创建获取账目列表存储过程
                NewQAccfList.Connection = Connect;//指定连接
                NewQAccfList.ExecuteNonQuery();//执行创建
                OracleCommand UpAccount = new OracleCommand(@"create or replace procedure UpAccount   
                                                            (v_AccId in varchar,
                                                            v_Goodsid in varchar,
                                                            v_GOODSNAME in varchar,
                                                            v_IN_PRICE in number,
                                                            v_OUT_PRICE in number,
                                                            v_OUTNUM in int,
                                                            v_UNIT in varchar,
                                                            v_Price in number)
                                                            is
                                                            begin
	                                                            update MarketTerminal_Account
                                                                 set GOODSNO=v_Goodsid,
                                                                     GOODSNAME=v_GOODSNAME,
                                                                     IN_PRICE=v_IN_PRICE,
                                                                     OUT_PRICE=v_OUT_PRICE,
                                                                     OUTNUM=v_OUTNUM,
                                                                     UNIT=v_UNIT,
                                                                     PRICE=v_Price
                                                                     where ACCOUNTNO=v_AccId;
                                                            end;");//创建更新账目存储过程
                UpAccount.Connection = Connect;//指定连接
                UpAccount.ExecuteNonQuery();//执行创建
                OracleCommand DeleteAccount = new OracleCommand(@"create or replace procedure DeleteAccount
                                                                (v_Acc in varchar)
                                                                is
                                                                begin
	                                                                delete from MarketTerminal_Account
	                                                                where ACCOUNTNO=v_Acc;
                                                                end;");//创建删除商品存储过程
                DeleteAccount.Connection = Connect;//指定连接
                DeleteAccount.ExecuteNonQuery();//执行创建
                OracleCommand NewQGoodsfList_Part = new OracleCommand(@"create or replace procedure proc_GetGoodsList_P
                                                            (v_Goodsid in varchar,
                                                                p_cur out PK_RefCur.p_cursor)
                                                            is
                                                            begin
	                                                            open p_cur for select * from MarketTerminal_Goods
                                                              where GoodsNo like '%'+v_Goodsid+'%';
                                                            end;");//创建获取模糊查找商品列表存储过程
                NewQGoodsfList_Part.Connection = Connect;//指定连接
                NewQGoodsfList_Part.ExecuteNonQuery();//执行创建
                OracleCommand GetNowFew = new OracleCommand(@"create or replace procedure proc_GetGoodsList_NF
                                                            (v_Thresh in integer,
                                                                p_cur out PK_RefCur.p_cursor)
                                                            is
                                                            begin
	                                                            open p_cur for select * from MarketTerminal_Goods
                                                              where Marketleft < v_Thresh;
                                                            end;");//创建获取货架余量不足的商品列表存储过程
                GetNowFew.Connection = Connect;//指定连接
                GetNowFew.ExecuteNonQuery();//执行创建
                OracleCommand GetTrunkFew = new OracleCommand(@"create or replace procedure proc_GetGoodsList_TF
                                                            (v_Thresh in integer,
                                                                p_cur out PK_RefCur.p_cursor)
                                                            is
                                                            begin
	                                                            open p_cur for select * from MarketTerminal_Goods
                                                              where TrunkLeft < v_Thresh;
                                                            end;");//创建获取总量不足的商品列表存储过程
                GetTrunkFew.Connection = Connect;//指定连接
                GetTrunkFew.ExecuteNonQuery();//执行创建

                return true;//创建成功
            }
            catch (Exception)
            {
                return false;//创建失败则返回false
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 删除员工表
        /// </summary>
        /// <returns>返回 true：成功 false：失败</returns>
        private Boolean DeleteStaffTbl()
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand DelStafftbl = new OracleCommand(@"drop table MarketTerminal_Staff purge");//删除员工表
                DelStafftbl.Connection = Connect;//指定连接
                DelStafftbl.ExecuteNonQuery();//执行删除表
                return true;//删除成功
            }
            catch (Exception)
            {
                return false;//失败则返回false
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 删除商品表
        /// </summary>
        /// <returns>返回 true：成功 false：失败</returns>
        private Boolean DeleteGoodsTbl()
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand DelGoodstbl = new OracleCommand(@"drop table MarketTerminal_Goods purge");//删除商品表
                DelGoodstbl.Connection = Connect;//指定连接
                DelGoodstbl.ExecuteNonQuery();//执行删除表
                return true;//删除成功
            }
            catch (Exception)
            {
                return false;//失败则返回false
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 删除账目表
        /// </summary>
        /// <returns>返回 true：成功 false：失败</returns>
        private Boolean DeleteAccountTbl()
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand DelAccounttbl = new OracleCommand(@"drop table MarketTerminal_Account purge");//删除账目表
                DelAccounttbl.Connection = Connect;//指定连接
                DelAccounttbl.ExecuteNonQuery();//执行删除表
                return true;//删除成功
            }
            catch (Exception)
            {
                return false;//失败则返回false
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 删除触发器
        /// </summary>
        /// <returns>返回 true：成功 false：失败</returns>
        private Boolean DeleteTrigger()
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand DelTrigger = new OracleCommand(@"drop trigger T_UpAcc");//删除触发器
                DelTrigger.Connection = Connect;//指定连接
                DelTrigger.ExecuteNonQuery();//执行删除
                return true;//删除成功
            }
            catch (Exception)
            {
                return false;//失败则返回false
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 删除游标与存储过程
        /// </summary>
        /// <returns></returns>
        private Boolean DeleteCurandProc()
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleCommand DelRC = new OracleCommand(@"drop package PK_RefCur");//删除引用游标
                DelRC.Connection = Connect;//指定连接
                DelRC.ExecuteNonQuery();//执行删除
                OracleCommand DelProc_GSL = new OracleCommand(@"drop procedure proc_GetStaffList");//删除获取员工列表的存储过程
                DelProc_GSL.Connection = Connect;//指定连接
                DelProc_GSL.ExecuteNonQuery();//执行删除
                OracleCommand DelProc_GGL = new OracleCommand(@"drop procedure proc_GetGoodsList");//删除获取商品列表的存储过程
                DelProc_GGL.Connection = Connect;//指定连接
                DelProc_GGL.ExecuteNonQuery();//执行删除
                OracleCommand DelProc_CSf = new OracleCommand(@"drop procedure CheckStaff");//删除员工登录的存储过程
                DelProc_CSf.Connection = Connect;//指定连接
                DelProc_CSf.ExecuteNonQuery();//执行删除
                OracleCommand DelProc_CSU = new OracleCommand(@"drop procedure CheckSU");//删除检查用户类型的存储过程
                DelProc_CSf.Connection = Connect;//指定连接
                DelProc_CSf.ExecuteNonQuery();//执行删除
                OracleCommand DelProc_UPUserName = new OracleCommand(@"drop procedure UPUserName");//删除更新用户名的存储过程
                DelProc_CSf.Connection = Connect;//指定连接
                DelProc_CSf.ExecuteNonQuery();//执行删除
                OracleCommand DelProc_UPUserPwd = new OracleCommand(@"drop procedure UPUserPwd");//删除更新用户密码的存储过程
                DelProc_CSf.Connection = Connect;//指定连接
                DelProc_CSf.ExecuteNonQuery();//执行删除
                OracleCommand DelProc_SetSUsta = new OracleCommand(@"drop procedure SetSUsta");//删除更新SU状态的存储过程
                DelProc_CSf.Connection = Connect;//指定连接
                DelProc_CSf.ExecuteNonQuery();//执行删除
                OracleCommand DelProc_DeleteUser = new OracleCommand(@"drop procedure DeleteUser");//删除删除员工的存储过程
                DelProc_CSf.Connection = Connect;//指定连接
                DelProc_CSf.ExecuteNonQuery();//执行删除
                OracleCommand DelProc_GetStaff = new OracleCommand(@"drop procedure GetStaff");//删除获取员工信息的存储过程
                DelProc_CSf.Connection = Connect;//指定连接
                DelProc_CSf.ExecuteNonQuery();//执行删除
                OracleCommand DelProc_NewStaff = new OracleCommand(@"drop procedure NewStaff");//删除新建员工的存储过程
                DelProc_CSf.Connection = Connect;//指定连接
                DelProc_CSf.ExecuteNonQuery();//执行删除
                OracleCommand DelProc_DeleteGoods = new OracleCommand(@"drop procedure DeleteGoods");//删除删除商品的存储过程
                DelProc_CSf.Connection = Connect;//指定连接
                DelProc_CSf.ExecuteNonQuery();//执行删除
                OracleCommand DelProc_CheckGoods = new OracleCommand(@"drop procedure CheckGoods");//删除检查商品的存储过程
                DelProc_CSf.Connection = Connect;//指定连接
                DelProc_CSf.ExecuteNonQuery();//执行删除
                OracleCommand DelProc_UpGoodsNum = new OracleCommand(@"drop procedure UpGoodsNum");//删除商品数目的存储过程
                DelProc_CSf.Connection = Connect;//指定连接
                DelProc_CSf.ExecuteNonQuery();//执行删除
                OracleCommand DelProc_UpGoodsNowNum = new OracleCommand(@"drop procedure UpGoodsNowNum");//删除获取货架商品数量的存储过程
                DelProc_CSf.Connection = Connect;//指定连接
                DelProc_CSf.ExecuteNonQuery();//执行删除
                OracleCommand DelProc_NewGoods = new OracleCommand(@"drop procedure NewGoods");//删除创建商品的存储过程
                DelProc_CSf.Connection = Connect;//指定连接
                DelProc_CSf.ExecuteNonQuery();//执行删除
                OracleCommand DelProc_GetGoods = new OracleCommand(@"drop procedure GetGoods");//删除获取商品信息的存储过程
                DelProc_CSf.Connection = Connect;//指定连接
                DelProc_CSf.ExecuteNonQuery();//执行删除
                OracleCommand DelProc_UPGoods = new OracleCommand(@"drop procedure UPGoods");//删除更新商品的存储过程
                DelProc_CSf.Connection = Connect;//指定连接
                DelProc_CSf.ExecuteNonQuery();//执行删除
                OracleCommand DelProc_proc_GetAccountList = new OracleCommand(@"drop procedure proc_GetAccountList");//删除获取账目列表的存储过程
                DelProc_CSf.Connection = Connect;//指定连接
                DelProc_CSf.ExecuteNonQuery();//执行删除
                OracleCommand DelProc_UpAccount = new OracleCommand(@"drop procedure UpAccount");//删除更新账目的存储过程
                DelProc_CSf.Connection = Connect;//指定连接
                DelProc_CSf.ExecuteNonQuery();//执行删除
                OracleCommand DelProc_DeleteAccount = new OracleCommand(@"drop procedure DeleteAccount");//删除删除账目的存储过程
                DelProc_CSf.Connection = Connect;//指定连接
                DelProc_CSf.ExecuteNonQuery();//执行删除
                OracleCommand DelProc_proc_GetGoodsList_P = new OracleCommand(@"drop procedure proc_GetGoodsList_P");//删除获取匹配商品列表的存储过程
                DelProc_CSf.Connection = Connect;//指定连接
                DelProc_CSf.ExecuteNonQuery();//执行删除
                OracleCommand DelProc_proc_GetGoodsList_NF = new OracleCommand(@"drop procedure proc_GetGoodsList_NF");//删除获取货架存量不足的存储过程
                DelProc_CSf.Connection = Connect;//指定连接
                DelProc_CSf.ExecuteNonQuery();//执行删除
                OracleCommand DelProc_proc_GetGoodsList_TF = new OracleCommand(@"drop procedure proc_GetGoodsList_TF");//删除获取总量不足的存储过程
                DelProc_CSf.Connection = Connect;//指定连接
                DelProc_CSf.ExecuteNonQuery();//执行删除
                return true;//删除成功
            }
            catch (Exception)
            {
                return false;//失败则返回false
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 初始化数据库，包括数据库创建、表结构创建
        /// </summary>
        /// <returns>返回 true：初始化成功 false：初始化失败</returns>
        public Boolean DataBaseSetting()
        {
            if (CreateStaffTbl() == true)//员工表创建成功
            {
                if (CreateGoodsTbl() == true)//商品表创建成功
                {
                    if (CreateAccountTbl() == true)//创建账目表成功
                    {
                        if (CreateTrigger() == true)//创建触发器成功
                        {
                            if (CreateCurandProc() == true)
                                return true;
                            else
                            {//回滚账目表、商品表、员工表
                                DeleteAccountTbl();//删除账目表
                                DeleteStaffTbl();//删除员工表
                                DeleteGoodsTbl();//删除商品表
                                return false;
                            }
                        }
                        else
                            return false;
                    }
                    else
                    {//回滚商品表、员工表
                        DeleteStaffTbl();//删除员工表
                        DeleteGoodsTbl();//删除商品表
                        return false;
                    }
                }
                else
                {//回滚员工表
                    DeleteStaffTbl();//删除员工表
                    return false;
                }
            }
            else
                return false;//员工表创建不成功，直接失败
        }
        /// <summary> 删除数据库
        /// </summary>
        /// <returns>返回 true：删除成功 false：删除失败</returns>
        public Boolean DataBaseDelete()
        {
            DeleteStaffTbl();//删除员工表
            DeleteTrigger();//删除触发器
            DeleteGoodsTbl();//删除商品表
            DeleteAccountTbl();//删除账目表
            DeleteCurandProc();//删除引用游标与存储过程
            return true;
        }
        /// <summary> 创建配置文件，存放连接参数
        /// </summary>
        public void CreateConfigFile()
        {
            String[] Config_Line = {User,Password};//配置行
            File.WriteAllLines(Environment.CurrentDirectory + "/Config.inf",Config_Line,Encoding.UTF8);//向配置文件写入配置行
        }
        /// <summary> 删除配置文件
        /// </summary>
        public void DeleteConfigFile()
        {
            File.Delete(Environment.CurrentDirectory + "/Config.inf");//删除根目录下的配置文件
        }
        /// <summary> 获得员工列表
        /// </summary>
        /// <returns>返回 员工列表</returns>
        public List<String[]> GetStaffList()
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            List<String[]> StaffList = new List<string[]>();//初始化员工列表
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库

                OracleParameter[] Parm = new OracleParameter[1];//实例化参数列表
                Parm[0] = new OracleParameter("p_cur",OracleType.Cursor);
                Parm[0].Direction = ParameterDirection.Output;//定义引用游标输出参数

                OracleCommand QueryStaffList = new OracleCommand("proc_GetStaffList",Connect);//指定存储过程
                QueryStaffList.CommandType = CommandType.StoredProcedure;//本次查询为存储过程
                QueryStaffList.Parameters.Clear();//清空参数列表
                foreach (OracleParameter tP in Parm)
                {//填充参数列表
                    QueryStaffList.Parameters.Add(tP);
                }
                OracleDataAdapter OA = new OracleDataAdapter(QueryStaffList);
                DataTable datatable = new DataTable();
                OA.Fill(datatable);//调用存储过程并拉取数据
                int i = datatable.Rows.Count;//循环行数次
                while ((i--) != 0)//按行读取，直到结尾
                {
                    StaffList.Add(new String[] {datatable.Rows[i][0].ToString(),//员工号
                                                datatable.Rows[i][1].ToString(),//姓名
                                                datatable.Rows[i][2].ToString() == "1"?"是":"否",//是否超级管理员
                                                datatable.Rows[i][3].ToString()});//密码
                }
                return StaffList;//查询成功
            }
            catch (Exception)
            {
                return null;//查询员工表失败则返回null
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 获得商品列表
        /// </summary>
        /// <returns>返回 商品列表</returns>
        public List<String[]> GetGoodsList()
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            List<String[]> GoodsList = new List<string[]>();//初始化商品列表
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库

                OracleParameter[] Parm = new OracleParameter[1];//实例化参数列表
                Parm[0] = new OracleParameter("p_cur", OracleType.Cursor);
                Parm[0].Direction = ParameterDirection.Output;//定义引用游标输出参数

                OracleCommand QueryGoodsList = new OracleCommand("proc_GetGoodsList", Connect);//指定存储过程
                QueryGoodsList.CommandType = CommandType.StoredProcedure;//本次查询为存储过程
                QueryGoodsList.Parameters.Clear();//清空参数列表
                foreach (OracleParameter tP in Parm)
                {//填充参数列表
                    QueryGoodsList.Parameters.Add(tP);
                }
                OracleDataAdapter OA = new OracleDataAdapter(QueryGoodsList);
                DataTable datatable = new DataTable();
                OA.Fill(datatable);//调用存储过程并拉取数据
                int i = datatable.Rows.Count;//循环行数次
                while ((i--) != 0)//按行读取，直到结尾
                {
                    GoodsList.Add(new String[] {datatable.Rows[i][0].ToString(),//商品编号
                                                datatable.Rows[i][1].ToString(),//商品名
                                                datatable.Rows[i][2].ToString(),//进价
                                                datatable.Rows[i][3].ToString(),//售价
                                                datatable.Rows[i][4].ToString(),//货架剩余
                                                datatable.Rows[i][5].ToString(),//仓库剩余
                                                datatable.Rows[i][6].ToString(),//品牌
                                                datatable.Rows[i][7].ToString()});//单位
                }
                return GoodsList;//查询成功
            }
            catch (Exception)
            {
                return null;//查询商品表失败则返回null
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 验证员工工号与密码
        /// </summary>
        /// <param name="_User">员工工号</param>
        /// <param name="_Password">员工密码</param>
        /// <returns>返回 true：正确 false：错误</returns>
        public Boolean CheckStaff(String _User, String _Password)
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库

                OracleParameter[] Parm = new OracleParameter[3];//实例化参数列表
                Parm[0] = new OracleParameter("v_USER", OracleType.VarChar);
                Parm[0].Direction = ParameterDirection.Input;//输入参数：用户名
                Parm[0].Value = _User;
                Parm[1] = new OracleParameter("v_PWD", OracleType.VarChar);
                Parm[1].Direction = ParameterDirection.Input;//输入参数：密码
                Parm[1].Value = _Password;
                Parm[2] = new OracleParameter("v_Ret", OracleType.Int16);
                Parm[2].Direction = ParameterDirection.Output;//输出参数：结果1/0

                OracleCommand Chk = new OracleCommand("CheckStaff", Connect);//指定存储过程
                Chk.CommandType = CommandType.StoredProcedure;//本次查询为存储过程
                Chk.Parameters.Clear();//清空参数列表
                foreach (OracleParameter tP in Parm)
                {//填充参数列表
                    Chk.Parameters.Add(tP);
                }
                Chk.ExecuteNonQuery();//调用存储过程
                if (Parm[2].Value.ToString() == "0")
                    return false;//验证失败
                else
                    return true;//验证成功
            }
            catch (Exception)
            {
                return false;//验证失败则返回false
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 检查员工是否是超级管理员
        /// </summary>
        /// <param name="_User">员工工号</param>
        /// <returns>返回 true：是 false：否</returns>
        public Boolean IsSuperUser(String _User)
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库

                OracleParameter[] Parm = new OracleParameter[2];//实例化参数列表
                Parm[0] = new OracleParameter("v_USER", OracleType.VarChar);
                Parm[0].Direction = ParameterDirection.Input;//输入参数：用户名
                Parm[0].Value = _User;
                Parm[1] = new OracleParameter("v_Ret", OracleType.Int16);
                Parm[1].Direction = ParameterDirection.Output;//输出参数：结果1/0

                OracleCommand Chk = new OracleCommand("CheckSU", Connect);//指定存储过程
                Chk.CommandType = CommandType.StoredProcedure;//本次查询为存储过程
                Chk.Parameters.Clear();//清空参数列表
                foreach (OracleParameter tP in Parm)
                {//填充参数列表
                    Chk.Parameters.Add(tP);
                }
                Chk.ExecuteNonQuery();//调用存储过程
                if (Parm[1].Value.ToString() == "0")
                    return false;//验证普通用户
                else
                    return true;//验证超级管理员
            }
            catch (Exception)
            {
                return false;//查询失败返回false
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }

        }
        /// <summary> 更新员工姓名
        /// </summary>
        /// <param name="_StaffNo">员工工号</param>
        /// <param name="_NewName">员工新姓名</param>
        /// <returns>返回 true：成功 false：失败</returns>
        public Boolean UpdateStaffName(String _StaffNo, String _NewName)
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库

                OracleParameter[] Parm = new OracleParameter[2];//实例化参数列表
                Parm[0] = new OracleParameter("v_USERid", OracleType.VarChar);
                Parm[0].Direction = ParameterDirection.Input;//输入参数：用户ID
                Parm[0].Value = _StaffNo;
                Parm[1] = new OracleParameter("v_newUSER", OracleType.VarChar);
                Parm[1].Direction = ParameterDirection.Input;//输入参数：新用户名
                Parm[1].Value = _NewName;

                OracleCommand UP = new OracleCommand("UPUserName", Connect);//指定存储过程
                UP.CommandType = CommandType.StoredProcedure;//本次查询为存储过程
                UP.Parameters.Clear();//清空参数列表
                foreach (OracleParameter tP in Parm)
                {//填充参数列表
                    UP.Parameters.Add(tP);
                }
                if (UP.ExecuteNonQuery() == 0)//调用存储过程
                    return false;//更新失败
                else
                    return true;//更新成功
            }
            catch (Exception)
            {
                return false;//更新失败
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }

        }
        /// <summary> 更新员工密码
        /// </summary>
        /// <param name="_StaffNo">员工工号</param>
        /// <param name="_NewName">员工新密码</param>
        /// <returns>返回 true：成功 false：失败</returns>
        public Boolean UpdateStaffPwd(String _StaffNo, String _NewPassword)
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleParameter[] Parm = new OracleParameter[2];//实例化参数列表
                Parm[0] = new OracleParameter("v_USERid", OracleType.VarChar);
                Parm[0].Direction = ParameterDirection.Input;//输入参数：用户ID
                Parm[0].Value = _StaffNo;
                Parm[1] = new OracleParameter("v_newPWD", OracleType.VarChar);
                Parm[1].Direction = ParameterDirection.Input;//输入参数：新密码
                Parm[1].Value = _NewPassword;

                OracleCommand UP = new OracleCommand("UPUserPwd", Connect);//指定存储过程
                UP.CommandType = CommandType.StoredProcedure;//本次查询为存储过程
                UP.Parameters.Clear();//清空参数列表
                foreach (OracleParameter tP in Parm)
                {//填充参数列表
                    UP.Parameters.Add(tP);
                }
                if (UP.ExecuteNonQuery() == 0)//调用存储过程
                    return false;//更新失败
                else
                    return true;//更新成功
            }
            catch (Exception)
            {
                return false;//更新失败
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 更新员工超级管理员状态
        /// </summary>
        /// <param name="_GoodsNo">员工工号</param>
        /// <param name="_SUstatus">员工SU状态</param>
        /// <returns>返回 true：成功 false：失败</returns>
        public Boolean UpdateStaffPower(String _StaffNo,Boolean _SUstatus)
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库

                OracleParameter[] Parm = new OracleParameter[2];//实例化参数列表
                Parm[0] = new OracleParameter("v_USERid", OracleType.VarChar);
                Parm[0].Direction = ParameterDirection.Input;//输入参数：用户ID
                Parm[0].Value = _StaffNo;
                Parm[1] = new OracleParameter("v_SUstatus", OracleType.Int16);
                Parm[1].Direction = ParameterDirection.Input;//输入参数：新用户类型
                if (_SUstatus == true)
                    Parm[1].Value = 1;//赋予超级管理员
                else
                    Parm[1].Value = 0;//撤销超级管理员

                OracleCommand UP = new OracleCommand("SetSUsta", Connect);//指定存储过程
                UP.CommandType = CommandType.StoredProcedure;//本次查询为存储过程
                UP.Parameters.Clear();//清空参数列表
                foreach (OracleParameter tP in Parm)
                {//填充参数列表
                    UP.Parameters.Add(tP);
                }
                if (UP.ExecuteNonQuery() == 0)//调用存储过程
                    return false;//更新失败
                else
                    return true;//更新成功
            }
            catch (Exception)
            {
                return false;//更新失败
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 删除员工
        /// </summary>
        /// <param name="_StaffNo">员工工号</param>
        /// <returns>返回 true：成功 false：失败</returns>
        public Boolean DeleteStaff(String _StaffNo)
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                OracleParameter[] Parm = new OracleParameter[1];//实例化参数列表
                Parm[0] = new OracleParameter("v_USERid", OracleType.VarChar);
                Parm[0].Direction = ParameterDirection.Input;//输入参数：用户ID
                Parm[0].Value = _StaffNo;

                OracleCommand UP = new OracleCommand("DeleteUser", Connect);//指定存储过程
                UP.CommandType = CommandType.StoredProcedure;//本次查询为存储过程
                UP.Parameters.Clear();//清空参数列表
                foreach (OracleParameter tP in Parm)
                {//填充参数列表
                    UP.Parameters.Add(tP);
                }
                if (UP.ExecuteNonQuery() == 0)//调用存储过程
                    return false;//删除失败
                else
                    return true;//删除成功
            }
            catch (Exception)
            {
                return false;//删除失败
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 获取单个员工信息
        /// </summary>
        /// <param name="_StaffNo">员工工号</param>
        /// <returns>返回 String[]：员工信息 null：失败</returns>
        public String[] GetStaff(String _StaffNo)
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库

                OracleParameter[] Parm = new OracleParameter[2];//实例化参数列表
                Parm[0] = new OracleParameter("v_USERid", OracleType.VarChar);
                Parm[0].Direction = ParameterDirection.Input;
                Parm[0].Value = _StaffNo;//输入员工工号
                Parm[1] = new OracleParameter("p_cur", OracleType.Cursor);
                Parm[1].Direction = ParameterDirection.Output;//定义引用游标输出参数

                OracleCommand QueryStaff = new OracleCommand("GetStaff", Connect);//指定存储过程
                QueryStaff.CommandType = CommandType.StoredProcedure;//本次查询为存储过程
                QueryStaff.Parameters.Clear();//清空参数列表
                foreach (OracleParameter tP in Parm)
                {//填充参数列表
                    QueryStaff.Parameters.Add(tP);
                }
                OracleDataAdapter OA = new OracleDataAdapter(QueryStaff);
                DataTable datatable = new DataTable();
                OA.Fill(datatable);//调用存储过程并拉取数据
                if (datatable.Rows.Count != 0)
                    return new String[] { datatable.Rows[0][0].ToString(), datatable.Rows[0][1].ToString(), datatable.Rows[0][2].ToString(), datatable.Rows[0][3].ToString() };//查询成功
                else
                    return null;//无此员工返回null
            }
            catch (Exception)
            {
                return null;//失败则返回null
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }

        }
        /// <summary> 新增员工
        /// </summary>
        /// <param name="_StaffInfo">员工信息集</param>
        /// <returns>返回 true：成功 false：失败</returns>
        public Boolean AddStaff(String[] _StaffInfo)
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库

                OracleParameter[] Parm = new OracleParameter[4];//实例化参数列表
                Parm[0] = new OracleParameter("v_USERid", OracleType.VarChar);
                Parm[0].Direction = ParameterDirection.Input;//输入参数：用户ID
                Parm[0].Value = _StaffInfo[0];
                Parm[1] = new OracleParameter("v_UserName", OracleType.VarChar);
                Parm[1].Direction = ParameterDirection.Input;//输入参数：用户名
                Parm[1].Value = _StaffInfo[1];
                Parm[2] = new OracleParameter("v_SU", OracleType.Int16);
                Parm[2].Direction = ParameterDirection.Input;//输入参数：用户SU状态
                if (_StaffInfo[2].Equals("是"))//有超级管理员
                    Parm[2].Value = 1;
                else//无超级管理员
                    Parm[2].Value = 0;
                Parm[3] = new OracleParameter("v_UserPWD", OracleType.VarChar);
                Parm[3].Direction = ParameterDirection.Input;//输入参数：用户密码
                Parm[3].Value = _StaffInfo[3];

                OracleCommand UP = new OracleCommand("NewStaff", Connect);//指定存储过程
                UP.CommandType = CommandType.StoredProcedure;//本次查询为存储过程
                UP.Parameters.Clear();//清空参数列表
                foreach (OracleParameter tP in Parm)
                {//填充参数列表
                    UP.Parameters.Add(tP);
                }
                if (UP.ExecuteNonQuery() == 0)//调用存储过程
                    return false;//删除失败
                else
                    return true;//删除成功
            }
            catch (Exception)
            {
                return false;//添加失败
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 删除商品
        /// </summary>
        /// <param name="_GoodsNo">商品编号</param>
        /// <returns>返回 true：成功 false：失败</returns>
        public Boolean DeleteGoods(String _GoodsNo)
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库

                OracleParameter[] Parm = new OracleParameter[1];//实例化参数列表
                Parm[0] = new OracleParameter("v_Goodsid", OracleType.VarChar);
                Parm[0].Direction = ParameterDirection.Input;//输入参数：商品号
                Parm[0].Value = _GoodsNo;

                OracleCommand UP = new OracleCommand("DeleteGoods", Connect);//指定存储过程
                UP.CommandType = CommandType.StoredProcedure;//本次查询为存储过程
                UP.Parameters.Clear();//清空参数列表
                foreach (OracleParameter tP in Parm)
                {//填充参数列表
                    UP.Parameters.Add(tP);
                }
                if (UP.ExecuteNonQuery() == 0)//调用存储过程
                    return false;//删除失败
                else
                    return true;//删除成功
            }
            catch (Exception)
            {
                return false;//删除失败
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 检查商品是否存在
        /// </summary>
        /// <param name="_GoodsNo">商品编号</param>
        /// <returns>返回 true：是 false：否</returns>
        public Boolean IsGoodsExists(String _GoodsNo)
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库

                OracleParameter[] Parm = new OracleParameter[2];//实例化参数列表
                Parm[0] = new OracleParameter("v_Goodsid", OracleType.VarChar);
                Parm[0].Direction = ParameterDirection.Input;//输入参数：商品名
                Parm[0].Value = _GoodsNo;
                Parm[1] = new OracleParameter("v_Ret", OracleType.Int16);
                Parm[1].Direction = ParameterDirection.Output;//输出参数：结果1/0

                OracleCommand Chk = new OracleCommand("CheckGoods", Connect);//指定存储过程
                Chk.CommandType = CommandType.StoredProcedure;//本次查询为存储过程
                Chk.Parameters.Clear();//清空参数列表
                foreach (OracleParameter tP in Parm)
                {//填充参数列表
                    Chk.Parameters.Add(tP);
                }
                Chk.ExecuteNonQuery();//调用存储过程
                if (Parm[1].Value.ToString() == "0")
                    return false;//无此类商品
                else
                    return true;//有此类商品
            }
            catch (Exception)
            {
                return false;//不存在
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 更新商品数量
        /// </summary>
        /// <param name="_GoodsNo">商品编号</param>
        /// <param name="Num">新增数</param>
        /// <returns>返回 true：成功 false：失败</returns>
        public Boolean UpdateGoodsNum(String _GoodsNo, int Num)
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库

                OracleParameter[] Parm = new OracleParameter[2];//实例化参数列表
                Parm[0] = new OracleParameter("v_Goodsid", OracleType.VarChar);
                Parm[0].Direction = ParameterDirection.Input;//输入参数：商品号
                Parm[0].Value = _GoodsNo;
                Parm[1] = new OracleParameter("v_Num", OracleType.VarChar);
                Parm[1].Direction = ParameterDirection.Input;//输入参数：新增数量
                Parm[1].Value = Num;

                OracleCommand UP = new OracleCommand("UpGoodsNum", Connect);//指定存储过程
                UP.CommandType = CommandType.StoredProcedure;//本次查询为存储过程
                UP.Parameters.Clear();//清空参数列表
                foreach (OracleParameter tP in Parm)
                {//填充参数列表
                    UP.Parameters.Add(tP);
                }
                if (UP.ExecuteNonQuery() == 0)//调用存储过程
                    return false;//更新失败
                else
                    return true;//更新成功
            }
            catch (Exception)
            {
                return false;//更新失败
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 更新商品数量_货架存量
        /// </summary>
        /// <param name="_GoodsNo">商品编号</param>
        /// <param name="Num">新增数</param>
        /// <returns>返回 true：成功 false：失败</returns>
        public Boolean UpdateGoodsNum_MarketLeft(String _GoodsNo, int Num)
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库

                OracleParameter[] Parm = new OracleParameter[2];//实例化参数列表
                Parm[0] = new OracleParameter("v_Goodsid", OracleType.VarChar);
                Parm[0].Direction = ParameterDirection.Input;//输入参数：商品号
                Parm[0].Value = _GoodsNo;
                Parm[1] = new OracleParameter("v_Num", OracleType.VarChar);
                Parm[1].Direction = ParameterDirection.Input;//输入参数：新增货架数量
                Parm[1].Value = Num;

                OracleCommand UP = new OracleCommand("UpGoodsNowNum", Connect);//指定存储过程
                UP.CommandType = CommandType.StoredProcedure;//本次查询为存储过程
                UP.Parameters.Clear();//清空参数列表
                foreach (OracleParameter tP in Parm)
                {//填充参数列表
                    UP.Parameters.Add(tP);
                }
                if (UP.ExecuteNonQuery() == 0)//调用存储过程
                    return false;//更新失败
                else
                    return true;//更新成功
            }
            catch (Exception)
            {
                return false;//更新失败
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 新增商品
        /// </summary>
        /// <param name="_StaffInfo">商品信息集</param>
        /// <returns>返回 true：成功 false：失败</returns>
        public Boolean AddGoods(String[] _GoodsInfo)
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库

                OracleParameter[] Parm = new OracleParameter[8];//实例化参数列表
                Parm[0] = new OracleParameter("v_Goodsid", OracleType.VarChar);
                Parm[0].Direction = ParameterDirection.Input;//输入参数：商品号
                Parm[0].Value = _GoodsInfo[0];
                Parm[1] = new OracleParameter("v_GOODSNAME", OracleType.VarChar);
                Parm[1].Direction = ParameterDirection.Input;//输入参数：商品名
                Parm[1].Value = _GoodsInfo[1];
                Parm[2] = new OracleParameter("v_IN_PRICE", OracleType.Number);
                Parm[2].Direction = ParameterDirection.Input;//输入参数：进价
                Parm[2].Value = _GoodsInfo[2];
                Parm[3] = new OracleParameter("v_OUT_PRICE", OracleType.Number);
                Parm[3].Direction = ParameterDirection.Input;//输入参数：售价
                Parm[3].Value = _GoodsInfo[3];
                Parm[4] = new OracleParameter("v_MARKETLEFT", OracleType.Int16);
                Parm[4].Direction = ParameterDirection.Input;//输入参数：超市余量
                Parm[4].Value = _GoodsInfo[4];
                Parm[5] = new OracleParameter("v_TRUNKLEFT", OracleType.Int16);
                Parm[5].Direction = ParameterDirection.Input;//输入参数：仓库余量
                Parm[5].Value = _GoodsInfo[5];
                Parm[6] = new OracleParameter("v_BRAND", OracleType.VarChar);
                Parm[6].Direction = ParameterDirection.Input;//输入参数：品牌
                Parm[6].Value = _GoodsInfo[6];
                Parm[7] = new OracleParameter("v_UNIT", OracleType.VarChar);
                Parm[7].Direction = ParameterDirection.Input;//输入参数：单位
                Parm[7].Value = _GoodsInfo[7];

                OracleCommand UP = new OracleCommand("NewGoods", Connect);//指定存储过程
                UP.CommandType = CommandType.StoredProcedure;//本次查询为存储过程
                UP.Parameters.Clear();//清空参数列表
                foreach (OracleParameter tP in Parm)
                {//填充参数列表
                    UP.Parameters.Add(tP);
                }
                if (UP.ExecuteNonQuery() == 0)//调用存储过程
                    return false;//删除失败
                else
                    return true;//删除成功
            }
            catch (Exception)
            {
                return false;//添加失败
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 获取单个商品信息
        /// </summary>
        /// <param name="_GoodsNo">商品编号</param>
        /// <returns>返回 String[]：商品信息 null：失败</returns>
        public String[] GetGoodsInfo(String _GoodsNo)
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库

                OracleParameter[] Parm = new OracleParameter[2];//实例化参数列表
                Parm[0] = new OracleParameter("v_Goodsid", OracleType.VarChar);
                Parm[0].Direction = ParameterDirection.Input;
                Parm[0].Value = _GoodsNo;//输入商品号
                Parm[1] = new OracleParameter("p_cur", OracleType.Cursor);
                Parm[1].Direction = ParameterDirection.Output;//定义引用游标输出参数

                OracleCommand QueryGoods = new OracleCommand("GetGoods", Connect);//指定存储过程
                QueryGoods.CommandType = CommandType.StoredProcedure;//本次查询为存储过程
                QueryGoods.Parameters.Clear();//清空参数列表
                foreach (OracleParameter tP in Parm)
                {//填充参数列表
                    QueryGoods.Parameters.Add(tP);
                }
                OracleDataAdapter OA = new OracleDataAdapter(QueryGoods);
                DataTable datatable = new DataTable();
                OA.Fill(datatable);//调用存储过程并拉取数据
                if (datatable.Rows.Count != 0)
                    return new String[] { datatable.Rows[0][0].ToString(), datatable.Rows[0][1].ToString(), datatable.Rows[0][2].ToString(), datatable.Rows[0][3].ToString(), datatable.Rows[0][4].ToString(), datatable.Rows[0][5].ToString(), datatable.Rows[0][6].ToString(), datatable.Rows[0][7].ToString() };//查询成功
                else
                    return null;//无此商品返回null
            }
            catch (Exception)
            {
                return null;//失败则返回null
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 更新商品信息
        /// </summary>
        /// <param name="_GoodsInfo">商品信息集</param>
        /// <returns>返回 true：成功 false：失败</returns>
        public Boolean UpdateGoodsInfo(String[] _GoodsInfo)
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库

                OracleParameter[] Parm = new OracleParameter[8];//实例化参数列表
                Parm[0] = new OracleParameter("v_Goodsid", OracleType.VarChar);
                Parm[0].Direction = ParameterDirection.Input;//输入参数：商品号
                Parm[0].Value = _GoodsInfo[0];
                Parm[1] = new OracleParameter("v_GOODSNAME", OracleType.VarChar);
                Parm[1].Direction = ParameterDirection.Input;//输入参数：商品名
                Parm[1].Value = _GoodsInfo[1];
                Parm[2] = new OracleParameter("v_IN_PRICE", OracleType.Number);
                Parm[2].Direction = ParameterDirection.Input;//输入参数：进价
                Parm[2].Value = _GoodsInfo[2];
                Parm[3] = new OracleParameter("v_OUT_PRICE", OracleType.Number);
                Parm[3].Direction = ParameterDirection.Input;//输入参数：售价
                Parm[3].Value = _GoodsInfo[3];
                Parm[4] = new OracleParameter("v_MARKETLEFT", OracleType.Int16);
                Parm[4].Direction = ParameterDirection.Input;//输入参数：超市余量
                Parm[4].Value = _GoodsInfo[4];
                Parm[5] = new OracleParameter("v_TRUNKLEFT", OracleType.Int16);
                Parm[5].Direction = ParameterDirection.Input;//输入参数：仓库余量
                Parm[5].Value = _GoodsInfo[5];
                Parm[6] = new OracleParameter("v_BRAND", OracleType.VarChar);
                Parm[6].Direction = ParameterDirection.Input;//输入参数：品牌
                Parm[6].Value = _GoodsInfo[6];
                Parm[7] = new OracleParameter("v_UNIT", OracleType.VarChar);
                Parm[7].Direction = ParameterDirection.Input;//输入参数：单位
                Parm[7].Value = _GoodsInfo[7];

                OracleCommand UP = new OracleCommand("UPGoods", Connect);//指定存储过程
                UP.CommandType = CommandType.StoredProcedure;//本次查询为存储过程
                UP.Parameters.Clear();//清空参数列表
                foreach (OracleParameter tP in Parm)
                {//填充参数列表
                    UP.Parameters.Add(tP);
                }
                if (UP.ExecuteNonQuery() == 0)//调用存储过程
                    return false;//删除失败
                else
                    return true;//删除成功
            }
            catch (Exception)
            {
                return false;//修改失败
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 获得账目列表
        /// </summary>
        /// <returns>返回 账目列表</returns>
        public List<String[]> GetAccountList()
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            List<String[]> AccountList = new List<string[]>();//初始化账目列表
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库

                OracleParameter[] Parm = new OracleParameter[1];//实例化参数列表
                Parm[0] = new OracleParameter("p_cur", OracleType.Cursor);
                Parm[0].Direction = ParameterDirection.Output;//定义引用游标输出参数

                OracleCommand QueryAccountList = new OracleCommand("proc_GetAccountList", Connect);//指定存储过程
                QueryAccountList.CommandType = CommandType.StoredProcedure;//本次查询为存储过程
                QueryAccountList.Parameters.Clear();//清空参数列表
                foreach (OracleParameter tP in Parm)
                {//填充参数列表
                    QueryAccountList.Parameters.Add(tP);
                }
                OracleDataAdapter OA = new OracleDataAdapter(QueryAccountList);
                DataTable datatable = new DataTable();
                OA.Fill(datatable);//调用存储过程并拉取数据
                int i = datatable.Rows.Count;//循环行数次
                while ((i--) != 0)//按行读取，直到结尾
                {
                    AccountList.Add(new String[] {datatable.Rows[i][0].ToString(),//账目流水号
                                                datatable.Rows[i][1].ToString(),//商品号
                                                datatable.Rows[i][2].ToString(),//商品名
                                                datatable.Rows[i][3].ToString(),//进价
                                                datatable.Rows[i][4].ToString(),//售价
                                                datatable.Rows[i][5].ToString(),//售出量
                                                datatable.Rows[i][6].ToString(),//单位
                                                datatable.Rows[i][7].ToString()});//利润
                }
                return AccountList;//查询成功
            }
            catch (Exception)
            {
                return null;//查询账目表失败则返回null
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 更新账目信息
        /// </summary>
        /// <param name="_GoodsInfo">账目信息集</param>
        /// <returns>返回 true：成功 false：失败</returns>
        public Boolean UpdateAccountInfo(String[] _AccountInfo)
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库

                OracleParameter[] Parm = new OracleParameter[8];//实例化参数列表
                Parm[0] = new OracleParameter("v_AccId", OracleType.VarChar);
                Parm[0].Direction = ParameterDirection.Input;//输入参数：账目号
                Parm[0].Value = _AccountInfo[0];
                Parm[1] = new OracleParameter("v_Goodsid", OracleType.VarChar);
                Parm[1].Direction = ParameterDirection.Input;//输入参数：商品号
                Parm[1].Value = _AccountInfo[1];
                Parm[2] = new OracleParameter("v_GOODSNAME", OracleType.VarChar);
                Parm[2].Direction = ParameterDirection.Input;//输入参数：商品名
                Parm[2].Value = _AccountInfo[2];
                Parm[3] = new OracleParameter("v_IN_PRICE", OracleType.Number);
                Parm[3].Direction = ParameterDirection.Input;//输入参数：进价
                Parm[3].Value = _AccountInfo[3];
                Parm[4] = new OracleParameter("v_OUT_PRICE", OracleType.Number);
                Parm[4].Direction = ParameterDirection.Input;//输入参数：售价
                Parm[4].Value = _AccountInfo[4];
                Parm[5] = new OracleParameter("v_OUTNUM", OracleType.Int16);
                Parm[5].Direction = ParameterDirection.Input;//输入参数：售出量
                Parm[5].Value = _AccountInfo[5];
                Parm[6] = new OracleParameter("v_UNIT", OracleType.VarChar);
                Parm[6].Direction = ParameterDirection.Input;//输入参数：单位
                Parm[6].Value = _AccountInfo[6];
                Parm[7] = new OracleParameter("v_Price", OracleType.Number);
                Parm[7].Direction = ParameterDirection.Input;//输入参数：利润
                Parm[7].Value = _AccountInfo[7];

                OracleCommand UP = new OracleCommand("UpAccount", Connect);//指定存储过程
                UP.CommandType = CommandType.StoredProcedure;//本次查询为存储过程
                UP.Parameters.Clear();//清空参数列表
                foreach (OracleParameter tP in Parm)
                {//填充参数列表
                    UP.Parameters.Add(tP);
                }
                if (UP.ExecuteNonQuery() == 0)//调用存储过程
                    return false;//删除失败
                else
                    return true;//删除成功
            }
            catch (Exception)
            {
                return false;//修改失败
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 删除账目
        /// </summary>
        /// <param name="_GoodsNo">账目编号</param>
        /// <returns>返回 true：成功 false：失败</returns>
        public Boolean DeleteAccount(String _AccountNo)
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库

                OracleParameter[] Parm = new OracleParameter[1];//实例化参数列表
                Parm[0] = new OracleParameter("v_Acc", OracleType.VarChar);
                Parm[0].Direction = ParameterDirection.Input;//输入参数：账目号
                Parm[0].Value = _AccountNo;

                OracleCommand UP = new OracleCommand("DeleteAccount", Connect);//指定存储过程
                UP.CommandType = CommandType.StoredProcedure;//本次查询为存储过程
                UP.Parameters.Clear();//清空参数列表
                foreach (OracleParameter tP in Parm)
                {//填充参数列表
                    UP.Parameters.Add(tP);
                }
                if (UP.ExecuteNonQuery() == 0)//调用存储过程
                    return false;//删除失败
                else
                    return true;//删除成功
            }
            catch (Exception)
            {
                return false;//删除失败
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 模糊查找商品编号
        /// </summary>
        /// <param name="_GoodsNo">商品编号(part)</param>
        /// <returns>返回 String[]：商品信息 null：失败</returns>
        public List<String[]> GetGoodsInfo_Part(String _GoodsNo)
        {
            List<String[]> InfoList = new List<string[]>();//找到的商品列表
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库

                OracleParameter[] Parm = new OracleParameter[2];//实例化参数列表
                Parm[0] = new OracleParameter("v_Goodsid", OracleType.VarChar);
                Parm[0].Direction = ParameterDirection.Input;//定义引用游标输出参数
                Parm[0].Value = _GoodsNo;
                Parm[1] = new OracleParameter("p_cur", OracleType.Cursor);
                Parm[1].Direction = ParameterDirection.Output;//定义引用游标输出参数

                OracleCommand QueryAccountList = new OracleCommand("proc_GetGoodsList_P", Connect);//指定存储过程
                QueryAccountList.CommandType = CommandType.StoredProcedure;//本次查询为存储过程
                QueryAccountList.Parameters.Clear();//清空参数列表
                foreach (OracleParameter tP in Parm)
                {//填充参数列表
                    QueryAccountList.Parameters.Add(tP);
                }
                OracleDataAdapter OA = new OracleDataAdapter(QueryAccountList);
                DataTable datatable = new DataTable();
                OA.Fill(datatable);//调用存储过程并拉取数据
                int i = datatable.Rows.Count;//循环行数次
                while ((i--) != 0)//按行读取，直到结尾
                {
                    InfoList.Add(new String[] {datatable.Rows[i][0].ToString(),//商品编号
                                                datatable.Rows[i][1].ToString(),//商品名
                                                datatable.Rows[i][2].ToString(),//进价
                                                datatable.Rows[i][3].ToString(),//售价
                                                datatable.Rows[i][4].ToString(),//货架剩余
                                                datatable.Rows[i][5].ToString(),//仓库剩余
                                                datatable.Rows[i][6].ToString(),//品牌
                                                datatable.Rows[i][7].ToString()});//单位
                }
                return InfoList;//查询成功
            }
            catch (Exception)
            {
                return null;//失败则返回null
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 查找货架存量不足的商品
        /// </summary>
        /// <param name="Threshold">阈值</param>
        /// <returns>返回 String[]：商品信息 null：失败</returns>
        public List<String[]> GetGoodsMarketleftNotOk(int Threshold = 10)
        {
            List<String[]> InfoList = new List<string[]>();//找到的商品列表
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库

                OracleParameter[] Parm = new OracleParameter[2];//实例化参数列表
                Parm[0] = new OracleParameter("v_Thresh", OracleType.Int16);
                Parm[0].Direction = ParameterDirection.Input;//阈值
                Parm[0].Value = Threshold;
                Parm[1] = new OracleParameter("p_cur", OracleType.Cursor);
                Parm[1].Direction = ParameterDirection.Output;//定义引用游标输出参数

                OracleCommand QueryAccountList = new OracleCommand("proc_GetGoodsList_NF", Connect);//指定存储过程
                QueryAccountList.CommandType = CommandType.StoredProcedure;//本次查询为存储过程
                QueryAccountList.Parameters.Clear();//清空参数列表
                foreach (OracleParameter tP in Parm)
                {//填充参数列表
                    QueryAccountList.Parameters.Add(tP);
                }
                OracleDataAdapter OA = new OracleDataAdapter(QueryAccountList);
                DataTable datatable = new DataTable();
                OA.Fill(datatable);//调用存储过程并拉取数据
                int i = datatable.Rows.Count;//循环行数次
                while ((i--) != 0)//按行读取，直到结尾
                {
                    InfoList.Add(new String[] {datatable.Rows[i][0].ToString(),//商品编号
                                                datatable.Rows[i][1].ToString(),//商品名
                                                datatable.Rows[i][2].ToString(),//进价
                                                datatable.Rows[i][3].ToString(),//售价
                                                datatable.Rows[i][4].ToString(),//货架剩余
                                                datatable.Rows[i][5].ToString(),//仓库剩余
                                                datatable.Rows[i][6].ToString(),//品牌
                                                datatable.Rows[i][7].ToString()});//单位
                }
                return InfoList;//查询成功
            }
            catch (Exception)
            {
                return null;//失败则返回null
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 查找库存量不足的商品
        /// </summary>
        /// <param name="Threshold">阈值</param>
        /// <returns>返回 String[]：商品信息 null：失败</returns>
        public List<String[]> GetGoodsTrunkleftNotOk(int Threshold = 50)
        {
            List<String[]> InfoList = new List<string[]>();//找到的商品列表
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库

                OracleParameter[] Parm = new OracleParameter[2];//实例化参数列表
                Parm[0] = new OracleParameter("v_Thresh", OracleType.Int16);
                Parm[0].Direction = ParameterDirection.Input;//阈值
                Parm[0].Value = Threshold;
                Parm[1] = new OracleParameter("p_cur", OracleType.Cursor);
                Parm[1].Direction = ParameterDirection.Output;//定义引用游标输出参数

                OracleCommand QueryAccountList = new OracleCommand("proc_GetGoodsList_TF", Connect);//指定存储过程
                QueryAccountList.CommandType = CommandType.StoredProcedure;//本次查询为存储过程
                QueryAccountList.Parameters.Clear();//清空参数列表
                foreach (OracleParameter tP in Parm)
                {//填充参数列表
                    QueryAccountList.Parameters.Add(tP);
                }
                OracleDataAdapter OA = new OracleDataAdapter(QueryAccountList);
                DataTable datatable = new DataTable();
                OA.Fill(datatable);//调用存储过程并拉取数据
                int i = datatable.Rows.Count;//循环行数次
                while ((i--) != 0)//按行读取，直到结尾
                {
                    InfoList.Add(new String[] {datatable.Rows[i][0].ToString(),//商品编号
                                                datatable.Rows[i][1].ToString(),//商品名
                                                datatable.Rows[i][2].ToString(),//进价
                                                datatable.Rows[i][3].ToString(),//售价
                                                datatable.Rows[i][4].ToString(),//货架剩余
                                                datatable.Rows[i][5].ToString(),//仓库剩余
                                                datatable.Rows[i][6].ToString(),//品牌
                                                datatable.Rows[i][7].ToString()});//单位
                }
                return InfoList;//查询成功
            }
            catch (Exception)
            {
                return null;//失败则返回null
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
        /// <summary> 检测数据库是否已成功初始化
        /// </summary>
        /// <returns></returns>
        public Boolean CheckDBReady()
        {
            String Connect_Str = GetConnectStr(User, Password);//获取数据库连接参数字符串
            OracleConnection Connect = new OracleConnection(Connect_Str);//实例化连接oracle类
            try
            {
                Connect.Open();//尝试连接数据库
                //若有表名包含marketterminal则代表已成功初始化
                OracleCommand Chktbl = new OracleCommand(@"select count(*)
                                                            from tabs
                                                            where table_name like 'MARKETTERMINAL_%'");//查询表语句
                Chktbl.Connection = Connect;//指定连接
                OracleDataReader Reader = Chktbl.ExecuteReader();//执行查询
                Reader.Read();//读取count
                if (int.Parse(Reader[0].ToString()) > 0)
                {
                    return true;//有相关表，初始化成功
                }
                else
                {
                    return false;//无相关表，初始化失败
                }
            }
            catch (Exception)
            {
                return false;//查询失败
            }
            finally
            {
                Connect.Close();//最后必须关闭数据库连接
            }
        }
    }
}
