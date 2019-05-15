For English:
# Coding_deal_py

Using this code complied by Python ,you can delete the useless code in you code file,speclally the Chinese comment,but can't delete the '//'(for C# program ),of course useful for any program code if you want to delete the Chinese comment! And  this is the first step, the code for deal  the code will be more  perfect in the furture!


For Chinese:

*****************************************************************
                     代码自动优化整理器
*****************************************************************

——————————————————————————————————
                      -- 20190416 --
——————————————————————————————————

1.代码中文去除，保留程序功能中文，注释性中文删除
2.在run.sh 中修改路径，目前程序中写死一次只批量处理3个文件路径文件
3.run.sh 示例：
  #!/bin/bash
  cd src_code
  python detel_main.py txtdata/ConsoleApp txtdata/manage txtdata/monitor_code
4.由于文件操作权限问题，在配置好python运行环境后，切换到 root 用户运行
  >> bash run.sh
5.具体问题详见-原创知乎文章：

——————————————————————————————————