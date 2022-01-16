using System;
using System.Collections;
using System.Collections.Generic;

//工具类
public static class Utility{

    //将数组打乱（洗牌算法）
    public static T[] ShuffleArray<T>(T[] array, int seed){

        System.Random rand = new System.Random(seed);//根据种子值seed获取随机数序列

        for(int i = 0; i < array.Length - 1; i++){
            int randomIndex = rand.Next(i,array.Length);
            T temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }

        return array;
    }
}
