using UnityEngine;
using VContainer;
using VContainer.Unity;

public class RuleInstaller : MonoBehaviour, IInstaller
{

    public void Install(IContainerBuilder builder)
    {
        // Rules
        builder.Register<NewOrbitScoreRule>(Lifetime.Singleton);
        builder.Register<EnemyDefeatedScoreRule>(Lifetime.Singleton);
        builder.Register<PointObjectScoreRule>(Lifetime.Singleton);

        builder.Register<PlayerDeadFinishRule>(Lifetime.Singleton);
        builder.Register<TimeUpFinishRule>(Lifetime.Singleton);

        // Engines
        builder.Register<ScoreRuleEngine>(Lifetime.Singleton);
        builder.Register<GameFinishRuleEngine>(Lifetime.Singleton);

        // Facades
        builder.Register<RuleFacade>(Lifetime.Singleton).As<IRuleFacade>();
        builder.Register<RuleExternalFacade>(Lifetime.Singleton).As<IRuleExternalFacade>();


    }
}

