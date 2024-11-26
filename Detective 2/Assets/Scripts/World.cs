using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class World
{
    public static bool knowNews; //мировые события, от которых зависит дальнешее повествование

    public static bool onPause;

    public static void SetPause()
    {
        ParticleSystem[] sceneParticleSystems = Object.FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None);
        Animator[] sceneAnimators = Object.FindObjectsByType<Animator>(FindObjectsSortMode.None);
        NavMeshAgent[] sceneNavMeshAgents = Object.FindObjectsByType<NavMeshAgent>(FindObjectsSortMode.None);
        switch (onPause)
        {
            case true:
                foreach (ParticleSystem _ps in sceneParticleSystems)
                {
                    _ps.Pause();
                }
                foreach (Animator _anims in sceneAnimators)
                {
                    _anims.speed = 0;
                }
                foreach (NavMeshAgent _nma in sceneNavMeshAgents)
                {
                    if (_nma.enabled)
                        _nma.isStopped = true;
                }
                break;
            case false:
                foreach (ParticleSystem _ps in sceneParticleSystems)
                {
                    _ps.Play();
                }
                foreach (Animator _anims in sceneAnimators)
                {
                    _anims.speed = 1;
                }
                foreach (NavMeshAgent _nma in sceneNavMeshAgents)
                {
                    if (_nma.enabled)
                        _nma.isStopped = false;
                }
                break;
        }
    }
}
