# encoding:utf-8
import tushare as ts
import pandas as pd
import pymysql
import datetime


config = {
          'host':'106.14.145.51',
          'port':4000,
          'user':'pic98',
          'password':'vck123456',
          'database':'Stocks',
          }

ts.set_token('0ec5e057cd24f22904e0bfe44001c4456837061d9871b204e808fc54')
pro = ts.pro_api()

#df = ts.get_today_ticks('600309')
#df.head(10)
#df = ts.get_tick_data('000001','2018-10-11',3,0,'tt')
df = pro.stock_basic()
print('-------')
db = pymysql.connect(**config)
cursor = db.cursor()

for dd in df.values:
    try:
        today=datetime.date.today()
        oneday=datetime.timedelta(days=1)
        li=[]
        for i in range(0,60):
            try:
                today=today-oneday
                ticktoday=datetime.datetime.strftime(today,'%Y-%m-%d')
                ticktodayid=datetime.datetime.strftime(today,'%Y%m%d')
                #print(ticktoday)

                sql = "SELECT dayid,ts_code,value FROM PowerByDay WHERE ts_code = '%s' order by dayid desc"
                data = (dd[0])
                cursor.execute(sql % data)
                if cursor.rowcount > 0:
                    result = cursor.fetchall()
                    continue_ = False
                    for item in result:
                        if item[0] == int(ticktodayid):
                            continue_ = True
                            break
                    if continue_ == True:
                        print(ticktodayid,dd[0],"已有跳过...")
                        continue

                tick = ts.get_tick_data(dd[1],ticktoday,3,0,'tt')
                #print(tick)
                if tick is None : 
                    continue
                powertotal = 0
                for tick0 in tick.values:
                    if tick0[5] == '卖盘' :
                        powertotal -= tick0[3]
                    elif tick0[5] == '买盘':
                        powertotal += tick0[3]

                sql = 'INSERT INTO PowerByDay(DayId,ts_code, symbol, name, market,value ) \
                VALUES (%s, "%s", "%s", "%s","%s",%.2f)' 
                data = (ticktodayid,dd[0],dd[1],dd[2],dd[5],powertotal)
                print(sql % data)
                cursor.execute(data)
                db.commit()
            except Exception as e:
                print(e)
                db.rollback()
    except Exception as e:
        print(e)
        
 
# 关闭数据库连接
db.close()



print('-------')
#df = pro.query('suspend', ts_code='', suspend_date='20180927', resume_date='', fiedls='')
#df = pro.suspend(ts_code='600848.SH', suspend_date='', resume_date='', fiedls='')
#print(df)
print(ts.__version__)