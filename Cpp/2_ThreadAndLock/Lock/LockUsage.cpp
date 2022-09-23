#include <iostream>
#include <thread>
#include <mutex>
#include <chrono>

#define MUTEX_1

std::mutex mutex_1;
int cnt_1 = 0;

std::mutex mutex_2;
int cnt_2 = 0;



#pragma region : 1. Mutex thread：互斥锁

void ThreadFuncA()
{
    // Add a lock, other thread will can't access "cnt_1".
    mutex_1.lock(); 

    // Do something in this lock scope.
    for (int i = 0; i < 5; ++i)
    {
        std::cout << std::this_thread::get_id() << " = " << cnt_1 <<std::endl ;
        ++cnt_1;
    }

    std::this_thread::sleep_for(std::chrono::seconds(2));
    // Unlock and make other thread can access and operate variable "cnt_1".
    mutex_1.unlock();
}

void ThreadFuncB()
{
    mutex_1.lock();
    for (int i = 0; i < 5; ++i)
    {
        std::cout << std::this_thread::get_id() << " ***** " << i << std::endl;
    }
    mutex_1.unlock();
}

/// <summary>
/// 使用 std::lock_guard 来创建一个互斥锁，
/// 该互斥锁只是在一个有限的作用域内生效，执行完之后会自动释放锁对象，
/// 无需再去释放，避免了忘记释放锁对象造成的死锁问题。
/// </summary>
void ThreadFuncC()
{
    // 在以下的作用域区块创建一个互斥锁
    std::lock_guard<std::mutex> myGuard(mutex_2);

    for (int i = 0; i < 6; ++i)
    {
        std::cout << std::this_thread::get_id() << ", cnt_2 = " << cnt_2 << std::endl;
        ++cnt_2;
    }

    // "mutex_2" is automatically released when lock goes out of the scope.
}


#pragma endregion


int main()
{

#ifdef MUTEX_1
    std::thread myThread_1(ThreadFuncA);
    std::thread myThread_2(ThreadFuncA);
    std::thread myThread_3(ThreadFuncC);
    std::thread myThread_4(ThreadFuncC);

    std::cout << "begin join thread." << std::endl;

    myThread_1.join();
    std::cout << "now in fun 01" << std::endl;
    myThread_2.join();
    std::cout << "Final location..." << std::endl;

    myThread_3.join();
    myThread_4.join();

#endif // MUTEX_1

    



    std::cin.get();
    return 0;
}





















