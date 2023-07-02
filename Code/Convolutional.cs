using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Accord;
using System.Reflection;
using System.Reflection.Metadata;
using Accord.Collections;

namespace Get_Text
{
    //核心函数数值类型
    public class typek
    {
        public float source;
        public static implicit operator typek(float s)
        {
            typek t = new typek();
            t.source = s;
            return t;
        }
        public static implicit operator typek(int s)
        {
            typek t = new typek();
            t.source = s;
            return t;
        }
        public static explicit operator float(typek v)
        {
            return v.source;
            //throw new NotImplementedException();
        }
        public static explicit operator int(typek v)
        {
            return (int)v.source;
        }
        public static explicit operator double(typek v)
        {
            return v.source;
        }
    }
    /// <summary>
    /// 多维度支持的权重核心类
    /// </summary>
    class Kernel
    {
        //维度
        public int dimension = 0;
        //各个维度的长度
        public List<typek> sizeArray = new List<typek>();
        //数据表
        public List<typek> dataList = new List<typek>();
        public Kernel() { }
        //一维对象的核心初始化
        public Kernel(List<typek> dataList)
        {
            this.dataList = dataList;
            this.sizeArray.Add(dataList.Count);
            this.dimension = 1;
        }
        /// <summary>
        /// 一至三维的核心初始化(维度)
        /// </summary>
        /// <param name="dimension">维度</param>
        public Kernel(int dimension)
        {
            switch(dimension)
            {
                case 1:
                    dataList.Add(1);
                    dataList.Add(1);
                    dataList.Add(1);
                    sizeArray.Add(dataList.Count);
                    break;
                case 2:
                    for(int i =0;i < 3*3;i++)
                    {
                        dataList.Add(1);
                    }
                    sizeArray.Add(3);
                    sizeArray.Add(3);
                    break;
                case 3:
                    for (int i = 0; i < 3 * 3 * 3; i++)
                    {
                        dataList.Add(1);
                    }
                    sizeArray.Add(3);
                    sizeArray.Add(3);
                    sizeArray.Add(3);
                    break;
                default:
                    dataList.Add(1);
                    dataList.Add(1);
                    dataList.Add(1);
                    sizeArray.Add(dataList.Count);
                    break;
            }
            Init(ref dimension, ref sizeArray, ref dataList);
        }
        //核心初始化
        //initialize
        public void Init(ref int dimension, ref List<typek> sizeArray, ref List<typek> dataList)
        {
            this.dimension = dimension;
            this.sizeArray = sizeArray;
            this.dataList = dataList;
        }
        /// <summary>
        /// 核心初始化-平均卷积(维度，卷积核大小，卷积核当前维度溢出大小)
        /// </summary>
        /// <param name="dimension"></param>
        /// <param name="sizeArray"></param>
        /// <param name="overFlow"></param>
        public void InitForTarget(ref int dimension, ref List<typek> sizeArray,ref List<typek> overFlow)
        {
            this.dimension = dimension;
            this.sizeArray = sizeArray;
            //核心数据
            List<typek> dataList = new List<typek>();
            //核心数据大小
            int outListSize = 0;
            //维度坐标索引
            List<int> dimensionIndex = new List<int>();
            //计算出核心数据大小，及初始化维度坐标索引
            for (int i=0;i < sizeArray.Count;i++)
            {
                outListSize *= (int)sizeArray[i];
                dimensionIndex.Add(0);
            }
            //初始化核心数据
            for (int i=0;i < outListSize;i++)
            {
                int lastIndex = dimensionIndex.Count - 1;
                dimensionIndex[lastIndex]++;
                //判断进位,求出当前索引位置的坐标
                while (dimensionIndex[lastIndex] > (float)sizeArray[lastIndex] - 1)
                {
                    dimensionIndex[lastIndex] = 0;
                    lastIndex--;
                    dimensionIndex[lastIndex]++;
                    if (lastIndex < 0)
                        break;
                    else if(lastIndex >= dimensionIndex.Count)
                        break;
                }
                float weight = 0;//权重
                float maxweight = 0;//最大权重
                for(int x=0;x < dimensionIndex.Count;x++)
                {
                    maxweight++;
                    //根据索引坐标位置求出,当前数据所对应的权重
                    if (dimensionIndex[x] <= 0 || dimensionIndex[x] >= (float)sizeArray[x])
                    {
                        //假如当前位置为卷积核边缘，根据溢出大小求出权重
                        weight += (float)overFlow[x] / 2.0f;
                    }
                    else
                    {
                        weight++;
                    }
                }
                dataList.Add(weight / maxweight);
            }
        }
        /// <summary>
        /// 获取坐标点在数据中的维度
        /// </summary>
        /// <param name="point">坐标点</param>
        /// <returns></returns>
        private int GetIndex(ref List<typek> point)
        {
            if (point.Count == dimension)
            {
                int index = 0;
                for (int i = 0; i < point.Count; i++)
                {
                    int single = 1;
                    for (int x = i; x < point.Count; x++)
                    {
                        single *= (int)sizeArray[x];
                    }
                    index += (int)point[i] * single;
                }
                return index;
            }
            return 0;
        }
        //一维对象的值获取
        public typek GetValue(int x)
        {
            return dataList[x];
        }
        //二维对象的值获取
        public typek GetValue(int x, int y)
        {
            List<typek> tmp = new List<typek>();
            tmp.Add(x);
            tmp.Add(y);
            int index = GetIndex(ref tmp);
            return dataList[index];
        }
        //三维对象的值获取
        public typek GetValue(int x,int y,int z)
        {
            List<typek> tmp = new List<typek>();
            tmp.Add(x);
            tmp.Add(y);
            tmp.Add(z);
            int index = GetIndex(ref tmp);
            return dataList[index];
        }
        //多维对象的值获取
        public typek GetValue(ref List<typek> point)
        {
            int index = GetIndex(ref point);
            return dataList[index];
        }
        public typek GetValueForIndex(int index)
        {
            if(index < dataList.Count)
                return dataList[index];
            return dataList[0];
        }
    }
    /// <summary>
    /// 多维度支持的卷积类
    /// </summary>
    class Convolutional
    {
        //维度
        private int dimension = 0;
        public int Dimension { get { dimension = sizeArray.Count; return dimension; }set { dimension = value; } }
        //输出的维度长度
        public List<typek> outSizeArray = new List<typek>();
        //各个维度的长度
        public List<typek> sizeArray = new List<typek>();
        //数据表
        public List<typek> dataList = new List<typek>();
        //卷积核心
        public Kernel kernel = new Kernel();
        //一维对象的卷积初始化
        public Convolutional(ref List<typek> dataList, ref int outSize)
        {
            this.dataList = dataList;
            this.sizeArray.Add(dataList.Count);
            this.Dimension = 1;
            this.outSizeArray.Add(outSize);
            kernel = new Kernel(1);
        }
        /// <summary>
        /// 卷积初始化
        /// </summary>
        /// <param name="dimension">维度</param>
        /// <param name="outSizeArray">输出数据大小</param>
        /// <param name="sizeArray">输入数据大小</param>
        /// <param name="dataList">输入数据</param>
        public Convolutional(ref int dimension, ref List<typek> outSizeArray, ref List<typek> sizeArray, ref List<typek> dataList)
        {
            Init(ref dimension,ref outSizeArray,ref sizeArray,ref dataList);
        }
        public void Init(ref int dimension,ref List<typek> outSizeArray, ref List<typek> sizeArray, ref List<typek> dataList)
        {
            this.Dimension = dimension;
            this.outSizeArray = outSizeArray;
            this.sizeArray = sizeArray;
            this.dataList= dataList;
            kernel = new Kernel(dimension);
        }
        private int GetIndex(ref List<typek> point)
        {
            if (point.Count == Dimension)
            {
                int index = 0;
                for (int i = 0; i < point.Count; i++)
                {
                    int single = 1;
                    for (int x = i; x < point.Count; x++)
                    {
                        single *= (int)sizeArray[x];
                    }
                    index += (int)point[i] * single;
                }
                return index;
            }
            return 0;
        }
        /// <summary>
        /// 单层的卷积运算 (卷积位置，卷积核心)
        /// </summary>
        /// <param name="sourcePoint"></param>
        /// <param name="sourceKernel"></param>
        /// <returns></returns>
        private typek SCov(ref List<int> sourcePoint, ref Kernel sourceKernel)
        {
            //for(int )
            //{
            //    //获取原图像数据中的索引值
            //    int sourceIndex = GetIndex(ref sourcePoint);
            //}
            List<typek> startPoint= new List<typek>();
            for (int i = 0; i < sourcePoint.Count; i++) 
            {
                int tmp = (int)(sourcePoint[i] - ((int)sourceKernel.sizeArray[i] / 2.0f));
                startPoint.Add(tmp);
            }
            int count = 0;
            typek result = 0;
            for (int i = 0; i < sourceKernel.sizeArray.Count; i++)
            {
                for (int x = 0; x < (int)sourceKernel.sizeArray[i]; x++)
                {
                    count++;
                    startPoint[i] = (typek)((int)startPoint[i] + 1);
                    int index = GetIndex(ref startPoint);
                    result = (float)result + (float)dataList[index];
                }
            }
            result = (float)result / count;
            return result;
        }
        //全卷积输出
        public List<typek> Cov(int index)
        {
            //输出数据
            List<typek> outlist = new List<typek>();
            //输出数据大小
            int outListSize = 0;
            //维度坐标索引
            List<int> dimensionIndex = new List<int>();
            //卷积数据范围
            List<typek> outCovSize = new List<typek>();
            //卷积数据超出的范围
            List<typek> outCovSizeOverFlow = new List<typek>();
            //求出输入数据大小,初始化维度坐标索引，计算出单次卷积运算所需要的范围
            for (int i = 0; i < outSizeArray.Count; i++)
            {
                outListSize *= (int)outSizeArray[i];
                dimensionIndex.Add(0);
                float tmp = (float)sizeArray[i] / (int)outSizeArray[i];
                //加入向上取整的卷积数据范围
                outCovSize.Add(MathF.Ceiling(tmp));
                //加入向下去整的卷积数据 范围溢出值
                outCovSizeOverFlow.Add(tmp - MathF.Floor(tmp));
            }
            //卷积核
            Kernel outKernel = new Kernel(dimension);
            //初始化卷积核大小
            outKernel.InitForTarget(ref dimension,ref outCovSize,ref outCovSizeOverFlow);
            //遍历所有输出数据
            for (int i = 0; i < outListSize; i++)
            {
                outlist.Add(0);
                int lastIndex = dimensionIndex.Count - 1;
                dimensionIndex[lastIndex]++;
                //判断进位
                while(dimensionIndex[lastIndex] > (float)outCovSize[lastIndex] - 1)
                {
                    dimensionIndex[lastIndex] = 0;
                    lastIndex--;
                    dimensionIndex[lastIndex]++;
                    if (lastIndex < 0)
                        break;
                    else if (lastIndex >= dimensionIndex.Count)
                        break;
                }
                //维度索引位置还原
                List<typek> dimensionReduction = new List<typek>();
                for(int x=0;x < dimensionIndex.Count;x++)
                {
                    //找到卷积位置所对应的原图像中的位置
                    dimensionReduction.Add((dimensionIndex[i] / (float)outCovSize[i]) * (float)sizeArray[i]);
                }
                //获取原图像数据中的索引值
                int sourceIndex = GetIndex(ref dimensionReduction);
                outlist[i] = SCov(ref dimensionIndex,ref outKernel);
            }
            return outlist;
        }
    }
}
