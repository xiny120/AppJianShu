# encoding:utf-8
import tushare as ts
import pandas as pd
import numpy as np
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

today=datetime.date.today()
ticktodayid=datetime.datetime.strftime(today,'%Y%m%d')

dicts = {}

pdb = pro.daily_basic(ts_code='', trade_date=ticktodayid, fields='ts_code,free_share')

for it in pdb.values:
    dicts[it[0]] = it[1]

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
        np.append(dd,dicts[dd[0]])
        #sql = "select sum(T.value) from (SELECT * FROM Stocks.PowerByDay where ts_code='%s' order by DayId desc limit 0 ,60) T"
        sql = "SELECT value FROM Stocks.PowerByDay where ts_code='%s' order by DayId desc limit 0 ,240"
        data = (dd[0])
        cursor.execute(sql % data)
        if cursor.rowcount > 0:
            result = cursor.fetchall()
            powercalc = 0
            idx = 0
            pc5 = 0
            pc10 = 0
            pc20 = 0
            pc60 = 0
            pc120 = 0
            pc240 = 0

            for item in result:
                if item[0] is None:
                    continue
                powercalc = item[0] + powercalc
                idx = idx + 1
                if idx == 5:
                    pc5 = powercalc
                elif idx == 10:
                    pc10 = powercalc
                elif idx == 20:
                    pc20 = powercalc
                elif idx == 60:
                    pc60 = powercalc
                elif idx == 120:
                    pc120 = powercalc
                elif idx == 240:
                    pc240 = powercalc

            ts_code = dd[0]
            sql = "INSERT INTO `Stocks`.`PowerCalc` (`ts_code`,`ts_name`,`ts_market`,`market`,`powercalc5`,`powercalc10`,`powercalc20`,`powercalc60`,`powercalc120`,`powercalc240`) VALUES('%s','%s','%s','%s',%f,%f,%f,%f,%f,%f);"
            data = (sql % (dd[0],dd[2],dd[5],ts_code[-2:],pc5,pc10,pc20,pc60,pc120,pc240))
            cursor.execute(data)
            print(data)
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