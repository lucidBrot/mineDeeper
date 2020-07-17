using Assets.Scripts.Data;

namespace Assets.Scripts.Solver.Rules
{
    public interface IHintRule : IRule
    {
        Hint GenerateHint(BoardCell consideredCell);
    }
}