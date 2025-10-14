using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;


/// <summary>
/// Gestione las animaciones de las diferentes partes del cuerpo y prendas de vestir de un jugador.
/// Permite cambiar las animaciones de la ropa dinámicamente y controlar las animaciones de movimiento.
/// </summary>
public class PlayerAnimation : MonoBehaviour
{
    #region Variables
    [SerializeField] private Animator _skinAnimator;
    [SerializeField] private Animator _hairAnimator;
    [SerializeField] private Animator _shirtAnimator;
    [SerializeField] private Animator _pantsAnimator;
    [SerializeField] private Animator _shoesAnimator;
    [SerializeField] private Animator _accesAnimator;
    #endregion

    /// <summary>
    /// Aplica diferentes controladores de animación (AnimatorOverrideController) a cada parte del cuerpo
    /// y prenda basándose en IDs individuales proporcionados.
    /// Esto permite "vestir" al personaje con diferentes apariencias y sus animaciones asociadas.
    /// Utiliza <see cref="SkinManager.Instance"/> para consultar los controladores.
    /// </summary>
    /// <param name="sk">ID de la piel.</param>
    /// <param name="hai">ID del cabello.</param>
    /// <param name="shi">ID de la playera.</param>
    /// <param name="pan">ID del pants.</param>
    /// <param name="sho">ID de lo zapatos.</param>
    /// <param name="acce">ID del accesorio.</param>
    public void GetDress(int sk, int hai, int shi, int pan, int sho, int acce)
    {
        _skinAnimator.runtimeAnimatorController = SkinManager.Instance.ConsultSkin(sk);
        _hairAnimator.runtimeAnimatorController = SkinManager.Instance.ConsultHair(hai);
        _shirtAnimator.runtimeAnimatorController = SkinManager.Instance.ConsultShirt(shi);
        _pantsAnimator.runtimeAnimatorController = SkinManager.Instance.ConsultPants(pan);
        _shoesAnimator.runtimeAnimatorController = SkinManager.Instance.ConsultShoes(sho);
        _accesAnimator.runtimeAnimatorController = SkinManager.Instance.ConsultAcce(acce);

    }
    
    /// <summary>
    /// Aplica diferentes controladores de animación a cada parte del cuerpo y prenda
    /// basándose en un objeto <see cref="SelectedClothes"/>.
    /// Esta es una sobrecarga de <see cref="GetDress(int,int,int,int,int,int)"/> para mayor comodidad.
    /// Utiliza <see cref="SkinManager.Instance"/> para consultar los controladores.
    /// </summary>
    /// <param name="sl">Un objeto <see cref="SelectedClothes"/></param> que contiene los IDs de las prendas seleccionadas.
    public void GetDress(SelectedClothes sl)
    {
        _skinAnimator.runtimeAnimatorController = SkinManager.Instance.ConsultSkin(sl.skin);
        _hairAnimator.runtimeAnimatorController = SkinManager.Instance.ConsultHair(sl.hair);
        _shirtAnimator.runtimeAnimatorController = SkinManager.Instance.ConsultShirt(sl.shirt);
        _pantsAnimator.runtimeAnimatorController = SkinManager.Instance.ConsultPants(sl.pants);
        _shoesAnimator.runtimeAnimatorController = SkinManager.Instance.ConsultShoes(sl.shoes);
        _accesAnimator.runtimeAnimatorController = SkinManager.Instance.ConsultAcce(sl.acce);
    }

    /// <summary>
    /// Controla los parámetros "Hor" (Horizontal) y "Ver" (Vertical) de todos los componentes Animator.
    /// </summary>
    /// <param name="Hor">Valor horizontal (ej. -1 para izquierda, 1 para derecha)</param>
    /// <param name="Ver">Vector vertical (ej. -1 para abajo, 1 para arriba)</param>
    public void Animations(float Hor, float Ver)
    {
        AnimFloats(_hairAnimator, Hor, Ver);
        AnimFloats(_skinAnimator, Hor, Ver);
        AnimFloats(_shirtAnimator, Hor, Ver);
        AnimFloats(_pantsAnimator, Hor, Ver);
        AnimFloats(_shoesAnimator, Hor, Ver);
        AnimFloats(_accesAnimator, Hor, Ver);
    }

    /// <summary>
    /// Dispara las animaciones de movimiento automáticas basándose en el tipo de dirección de escalera.
    /// </summary>
    /// <param name="st">El tipo de escalera (arriba, abajo, izquierda, derecha) que define la dirección
    /// de la animación.</param>
    public void AnimAuto(StairsType st)
    {
        switch (st)
        {
            case StairsType.Top:
                AnimationsAutoTop();
                break;
            case StairsType.Down:
                AnimationsAutoDown();
                break;
            case StairsType.Left:
                AnimationsAutoLeft();
                break;
            case StairsType.Right:
                AnimationsAutoRight();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Establece las animaciones para moverse hacia arriba.
    /// </summary>
    public void AnimationsAutoTop()
    {
        Animations(0f, 1);
    }
    
    /// <summary>
    /// Establece las animaciones para moverse hacia abajo.
    /// </summary>
    public void AnimationsAutoDown()
    {
        Animations(0f, -1);
    }
    
    /// <summary>
    /// Establece las animaciones para moverse a la derecha.
    /// </summary>
    public void AnimationsAutoRight()
    {

        Animations(1f, 0);
    }
    
    /// <summary>
    /// Establece las animaciones para moverse a la izquierda.
    /// </summary>
    public void AnimationsAutoLeft()
    {

        Animations(-1,0);
    }

    /// <summary>
    /// Estalece los valores de los parámetros "Ver" (vertical) y "Hor" (horizontal)
    /// en un componente Animator dado. Esto es crucial para controlar las animaciones de blend tree.
    /// </summary>
    /// <param name="anim">El componente Animator al que se le aplicarán los valores.</param>
    /// <param name="Hor">El valor para el parámetro "Hor" del Animator.</param>
    /// <param name="Ver">El valor para el parámetro "Ver" del Animator.</param>
    public void AnimFloats(Animator anim, float Hor, float Ver)
    {
        //Seteamos las variables de Ver y Hor del animator utilizados para el blend tree,
        //los ejes correspondientes de nuestro vector

        anim.SetFloat("Ver", Ver);
        anim.SetFloat("Hor", Hor);

    }

}
