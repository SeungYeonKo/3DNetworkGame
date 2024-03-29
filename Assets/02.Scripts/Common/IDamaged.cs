public interface IDamaged
{
    public void Damaged(int damage, int actorNumber);
    // actorID : 포톤 서버 내에서 유일한 식별자. 1번부터 시작. 들어오는 순서대로 부여받음
}