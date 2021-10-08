Update: The webapp is now live, though with api missing: http://83.66.137.46/personnel-tracking-webapp/

# Personnel tracking application with QR Code:
1. A construction company wants to track the entry-exit times of senior personnel in different regions at its construction sites.
2. One of the clients (a company) has 3000 people work at the construction site. 500 of those are personnel who undertake the duties of architects, engineers, department chiefs and construction chiefs. There is one CEO as well.
3. There are 15 zones within the construction site where different entrances and exits can be made.
4. Each senior personnel is responsible for the continuation and control of the works in each region and the follow-up of other personnel.
5. A company, would like to see, for example, when the engineer named Ali Kara entered and exited these 15 different regions during the day.
6. There is no requirement that the personnel will enter and exit every place every day. The difference is that they can enter each region on different days, or they can spend the whole day in only one region.
7. However, there are some personnel (for example, the site manager) who must enter and exit every area every day.
8. In addition, if some of the staff deserved an annual leave as the summer season is approaching, they also want to know when they are on annual leave so that there is no problem as they cannot enter and exit when they are on vacation.
9. The client does not understand of application, mobile application or anything; they know that if they have 3000 employees, 3000 of them use smartphones and their phones have cameras.
10. For these 15 regions, they create QR codes with company name, region name, latitude-longitude information, and paste them into the entrances of the regions by printing out A4. The staff makes entry and exit by reading that QR code.
11. We prepare a web application for them, a page opens, the staff writes the ID number on the page that opens, the camera turns on, the phone's camera reads the QR code, if it reads that QR code for the first time that day, it logs in, if it reads it a second time, it logs out.
12. Personnel can enter and exit the same area more than once during the day, no problem. But the client must know when he logged in and when he left. Also let's say the staff forgot to check out that day and went home. At the end of the day, the system should be able to log out those personnel automatically.
13. The client wants to report these entries and exits of the personnel on a monthly basis, get an excel printout, have the human resources send an e-mail automatically at the end of the month and have them uploaded to another personnel management application we use as outsourcing data. In these reports, they want to see information such as the region where the personnel entered and exited, the time, whether the system forgot to log out and logged out automatically, etc. The client only knows that the format of the output required for the personnel management system is JEYSIN 😊.
14. The company has a server, but it is not very fast, it is running MS-SQL on it and windows server 2016 is installed and has 4gb ram. The IT friend mentioned something called Dat Net Kor 😊, he said whether he installed it or not, what is it, tell the software friends that they will understand, you'll see what's going on.
15. The clinet's time is a bit limited. The works have started at the construction site and they want to start following the personnel quickly.
