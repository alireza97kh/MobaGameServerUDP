using Dobeil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroManager : SingletonBase<HeroManager>
{
    public HeroManagerScriptableObject heroManagerData;

	protected override void Awake()
	{
		base.Awake();
		heroManagerData.herosPrefab.Clear();
		foreach (var item in heroManagerData.savedHerosPrefab)
			heroManagerData.herosPrefab.Add(item.Id, item.hero);
	}
	public HeroControllerBase GetHeroPrefab(HeroId _heroId)
    {
        if (heroManagerData.herosPrefab.TryGetValue(_heroId, out HeroControllerBase result))
            return result;
        return null;
    }
}
