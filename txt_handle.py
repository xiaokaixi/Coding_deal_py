import re
import os

# 定义文本操作类
class txt_handle():

    # 定义文件path
    def __init__(self, path):
        self.path = path
        self.filst_cs = []
        self.filst_txt = []
        self.filt_code_str_list = []

    # 重命名函数
    def rename(self,type):
        if type == 'txt':
            file_list = os.listdir(self.path)
            total_num = len(file_list)
            for item_file in file_list:
                self.filst_cs.append(item_file)
                if item_file.endswith('.cs'):
                    src = os.path.join(os.path.abspath(self.path),item_file)
                    dst = os.path.join(os.path.abspath(self.path),item_file.split('.')[0]+'.txt')
                    self.filst_txt.append(item_file.split('.')[0]+'.txt')
                    try:
                        os.rename(src,dst)
                        print('converting %s to %s ...'%(src, dst))
                        # os.chmod(os.path.abspath(self.path) + '/' + item_file.split('.')[0] + '.txt', 0o777)  # 更改文件权限
                    except:
                        print('converting error ！')
                        continue
        if type == 'cs':
            for i in range(len(self.filst_cs)):
                src = os.path.join(os.path.abspath(self.path), self.filst_txt[i])
                dst = os.path.join(os.path.abspath(self.path), self.filst_cs[i])
                try:
                    os.rename(src, dst)
                    print('converting %s to %s ...' % (src, dst))
                except:
                    print('converting error ！')
                    continue
            return  self.filst_cs

    def replace_context(self):
        # 中文过滤函数
        # str_cheinese = str_cheinese.encode("utf-8")    #python系统内部就是Unicode编码，不必转换
        # filt_chinese = re.compile(u'[^\u4E00-\u9FA5]')  #滤出中文
        # filt_chinese = re.compile(u'[^\x00-\xff]')   #滤出英文、数字、字母大小写
        # 逐行读文件，替换中文
        for file_name in self.filst_txt:
            self.filt_code_str_list = []
            # with open(self.path+'\\'+file_name,'r+', encoding='utf-8') as file_obj:    # 适用于windows系统目录
            with open(self.path + '/' + file_name, 'r+', encoding='utf-8') as file_obj:  # 适用于linux系统目录
                for line in file_obj.readlines():
                    # 正则表达，匹配
                    if line.find('= "') >= 0 or line.find('MessageBox') >= 0:
                        list = line.split('\n\'')[0]
                        self.filt_code_str_list.append(list)
                        continue
                    filt_chinese = re.compile(u'[^\x00-\xff]')
                    filt_code_str = filt_chinese.sub(r'', str(line))  # replace
                    list = filt_code_str.split('\n\'')[0]
                    self.filt_code_str_list.append(list)
                file_obj.truncate()
                file_obj.close()
            # with open(self.path+'\\'+file_name, 'wb') as file_obj:   # 适用于windows系统目录
            with open(self.path + '/' + file_name, 'wb') as file_obj:  # 适用于linux系统目录
                for i in range(len(self.filt_code_str_list)):
                    file_obj.write((self.filt_code_str_list[i]).encode())
                file_obj.close()
