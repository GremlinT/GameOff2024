using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class World
{
    public static bool onPause;

    public static void SetPause(bool isPaused)
    {
        ParticleSystem[] sceneParticleSystems = Object.FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None);
        Animator[] sceneAnimators = Object.FindObjectsByType<Animator>(FindObjectsSortMode.None);
        NavMeshAgent[] sceneNavMeshAgents = Object.FindObjectsByType<NavMeshAgent>(FindObjectsSortMode.None);
        switch (isPaused)
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
                    _nma.isStopped = false;
                }
                break;
        }
    }
}
