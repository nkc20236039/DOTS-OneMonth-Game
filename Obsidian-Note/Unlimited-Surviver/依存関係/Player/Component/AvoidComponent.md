#StandardComponent 

回避を扱うコンポーネント

# パラメーター
float AvoidPower 回避の移動量
float AvoidingTime 回避の有効時間

# 変数
float3 AvoidDirection 回避方向
bool IsAvoiding 回避中か
float AvoidingElapsedTime 回避の経過時間

# 付随しているエンティティ
[[Player]]