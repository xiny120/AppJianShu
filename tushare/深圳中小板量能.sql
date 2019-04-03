SELECT ts_market,ts_code,ts_name,powercalc20 
FROM Stocks.PowerCalc 
where ts_market="中小板" 
order by powercalc20 desc;