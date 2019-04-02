# encoding:utf-8
import tushare as ts
import pandas as pd
import pymysql
import time
import datetime
import os

config = {
          'host':'106.14.145.51',
          'port':4000,
          'user':'pic98',
          'password':'vck123456',
          'database':'Stocks',
          }

ts.set_token('0ec5e057cd24f22904e0bfe44001c4456837061d9871b204e808fc54')
pro = ts.pro_api()
df = pro.stock_basic()
print('-------')
db = pymysql.connect(**config)
cursor = db.cursor()
#DELETE FROM `Stocks`.`PowerCalc` WHERE <{where_expression}>;

cursor.execute("delete from `Stocks`.`PowerCalc`")
idx = 0
for dd in df.values:
    idx = idx+1
    if idx % 50 == 0:
        os.system("cls")

    try:
        sql = "select sum(T.value) from (SELECT * FROM Stocks.PowerByDay where ts_code='%s' order by DayId desc limit 0 ,60) T"
        data = (dd[0])
        cursor.execute(sql % data)
        if cursor.rowcount > 0:
            result = cursor.fetchall()
            for item in result:
                powercalc = item[0]
                if powercalc is None:
                    powercalc = 0
                ts_code = dd[0]
                sql = "INSERT INTO `Stocks`.`PowerCalc` (`ts_code`,`ts_name`,`ts_market`,`powercalc`,`market`) VALUES('%s','%s','%s',%f,'%s');"
                cursor.execute(sql % (dd[0],dd[2],dd[5],powercalc,ts_code[-2:]))
                print(sql % (dd[0],dd[2],dd[5],powercalc,ts_code[-2:]))
                db.commit()


    except Exception as e:
        print(e)
        db.rollback()
        
 
# 关闭数据库连接
db.close()



print('-------')
#df = pro.query('suspend', ts_code='', suspend_date='20180927', resume_date='', fiedls='')
#df = pro.suspend(ts_code='600848.SH', suspend_date='', resume_date='', fiedls='')
#print(df)
print(ts.__version__)