import logging
import sys
import txt_handle

def main():
    # 初始化日志文件
    logging.basicConfig(level=logging.DEBUG,
                        filename='../logs/detel.log',
                        format= '[%(asctime)s] %(levelname)s [%(funcName)s: %(filename)s,%(lineno)d] %(message)s',
                        datefmt= '%Y-%m-%d %H:%M:%S',
                        filemode= 'a')
    # 获取文件路径
    file_count = 0
    if len(sys.argv) == 4:
        logging.info('delteling now ...')
        for k in range(1, 4):
            path = sys.argv[k]
            # 处理
            txt_my = txt_handle.txt_handle(path)
            txt_my.rename('txt')
            txt_my.replace_context()
            filename = txt_my.rename('cs')
            for i in range(len(filename)):
                logging.info(filename[i] + ' convert success!')
                print(filename[i] + ' convert success!')
                file_count += 1
        logging.info('Total cs file converted : %s ' % (file_count))
        print('Total cs file converted : %s ' % (file_count))
    else:
        logging.info('No path !!')
        print('No path !')
        exit(1)

# 主函数
if  __name__ == '__main__':
    main()

