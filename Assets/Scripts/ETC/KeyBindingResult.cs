using UnityEditor.PackageManager;

namespace KeyBinding
{
    public struct KeyBindingResult
    {
        public bool IsSuccess;
        public int ErrorType;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="errorType">0(유효하지 않은 키, 잘못된 키),  1(다른 키와 중복되는 키),-1(정상적으로 처리된 경우, 에러 아님)</param>
        public KeyBindingResult(bool isSuccess, int errorType=-1)
        {
            IsSuccess = isSuccess;
            ErrorType = errorType;
        }
    }
}

