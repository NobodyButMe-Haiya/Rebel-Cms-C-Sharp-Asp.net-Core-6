# RebelCmsTemplateSharp
Rebel Cms Template C Sharp

This is the third instalment from the C sharp CRUD Edition 

It shown how does a real single page application (SPA) and clean structure for building 
Asp.net .net Core 6 web application .

In this code you will learn out

1. Basic create , read , update ,delete , excel reporting within one single form 
2. Basic optimisation what need to be AJAX request  . 
3. Basic SQL optimisation for MySQL .

Custom Application  like Master Detail Not included here , we need more donation ,subscription on YouTube but.. Maybe future you can 
Get the code via patreon and with those example.

# We can give you an idea here. 

1. When creating first record at detail , you create a  master id which send back to form as hidden value , from there you can manipulate  to enter. 

# Why does SPA compare to  Normal Website 

1. Normal website do cache all those javascript , css and load lesser but loading time a bit higher. 
2. Normal javascript like vue , react load one time js file and maybe cache. We don’t like cache for some purpose.

# What will be odd  in this sample 
 1. XHR error - reason , the server load the data but not async. So don’t worry about it  because you tooo fast clicking. 

# Most people said vue,react and angular the best for asp.net for spa?
1. We no idea , we implement spa 10 years ago  without those thing . The last one using jquery same like this but no need those thing because not existed yet. 
2. The real spa is no website reload for all pages. Sorry we unsure how to manipulate hispter js library . We live in dom world not ref .
3. We can easily use resources like php for spa purpose . Razor and hot reload give a bit life
# What’s really problem in real life application moved to website. 

1. You have  10,000 product . A select box/ combo box in web wouldn’t handle this large data. If you need master-detail setup , rather clicked to open  to edit instead of one time add row  by row detail .  
2. Make a textbox suggest using datalist  or Ajax request on key type on TEXTBOX . Saving 10,000 dom is not possible and crash the website .(Maximum 500 record only datalist html)
3. Create a popup box search using twitter boostrap modal and send the dom id . upon select and search it will send back to the textbox the code instead of all information. Your server need check which code.(Northwind database sample refer to product code , i assume they had this bug issue upon large data) . 

Why still rename as Rebel ?
The name rebel is to make sure people understand we rebel for non structure development. Software development is long process hour and testing. A small company doesn’t understand CMMI or Waterfall only blaim to agile. A real business will be maintain if got stable based application. 

Are this rebelcms for commercial ?
Sorry not , if you deploy it commercial you need to contact us via curling_yaw.0w@icloud.com
If you want cheap solution , please find other . 

# How to install this 

1. Create a database rebelcms
2. Get this sql https://github.com/NobodyButMe-Haiya/RebelCmsTemplate/blob/master/RebelCmsTemplate/Database/1december2021_dummy_data and restore it 
3. From your terminal or command prompt run

```
dotnet watch run
```
* this is hot reload .net core 6 

5. To access the admin 

Username : admin
Password : 123456

** you may changed it at first time. 

**Youtube Preview**  
[![IMAGE ALT TEXT](http://img.youtube.com/vi/TgdJenkOLBY/0.jpg)](http://www.youtube.com/watch?v=xNLdBOmLr3g " Asp.net Core 6 Rebel CMS")

https://youtu.be/TgdJenkOLBY
