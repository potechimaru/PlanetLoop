
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

/// <summary>
/// Player�̌����ڂ�Transform�����S������View�N���X�B
///
/// ��Ɉȉ��̏�����S������B
///
/// �EPlayer�̃��[���h���W���X�V����
/// �E���ݏ������Ă���Spline��ێ�����
/// �E�`���[�W���x���ɉ����ăI�[���̃}�e���A����ύX����
/// �E�W�����v�����K�C�h��\���E��\���ɂ���
/// �ESpline���n���̃G�t�F�N�g���Đ�����
/// �EPlayer���S���̃G�t�F�N�g�Ə����A�j���[�V�������Đ�����
///
/// Player�̈ړ��v�Z��Q�[����̏�ԊǗ��͍s�킸�A
/// PlayerController��PlayerSplineMover����n���ꂽ���ʂ�
/// �����ڂ֔��f������������B
/// </summary>
public class PlayerView : MonoBehaviour
{
    /* =========================================================
     * Spline�֘A�ݒ�
     * ========================================================= */

    /// <summary>
    /// Player�����ݏ������Ă���Spline�B
    ///
    /// �Q�[���J�n����Inspector�Őݒ肳�ꂽSpline���g�p����A
    /// �ʂ�Spline�֒��n�����ۂ�SetSpline�ɂ���čX�V�����B
    /// </summary>
    [Header("Spline")]
    [SerializeField]
    private ClosedSplineLine _spline;

    /// <summary>
    /// Player�̈ړ����ʂƂ��āA
    /// ���[�J��XY���ʂ��g�p���邩�ǂ����B
    ///
    /// ���݂�PlayerView���ł͒��ڎg�p����Ă��Ȃ����A
    /// �O���N���X����UseLocalPlaneXY�v���p�e�B�o�R�ŎQ�Ƃł���B
    /// </summary>
    [SerializeField]
    private bool _useLocalPlaneXY = true;

    /// <summary>
    /// �`���[�W����Player�̃W�����v�\�������\������K�C�h�B
    ///
    /// Spline��̌��݈ʒu�ɂ�����O�����@��������
    /// ���o�I�ɕ\������B
    /// </summary>
    [SerializeField]
    private JumpNormalGuide _jumpNormalGuide;


    /* =========================================================
     * �I�[���p�}�e���A��
     * ========================================================= */

    /// <summary>
    /// �ʏ��ԂŎg�p����Player�I�[���̃}�e���A���B
    ///
    /// ChargeLevel.Normal�ɑΉ�����B
    /// </summary>
    [SerializeField]
    private Material _auraMaterialBlue;

    /// <summary>
    /// Charge1��ԂŎg�p����Player�I�[���̃}�e���A���B
    /// </summary>
    [SerializeField]
    private Material _auraMaterialOrange;

    /// <summary>
    /// Charge2��ԂŎg�p����Player�I�[���̃}�e���A���B
    /// </summary>
    [SerializeField]
    private Material _auraMaterialRed;


    /* =========================================================
     * ���S���o�ݒ�
     * ========================================================= */

    /// <summary>
    /// Player���S���ɍĐ�����ParticleSystem�B
    ///
    /// PlayDeadEffect���ōĐ�����A
    /// ParticleSystem�����S�ɏI������܂őҋ@����B
    /// </summary>
    [SerializeField]
    private ParticleSystem _deadEffect;

    /// <summary>
    /// Player�{�̂��\���֕ω������鎀�S�A�j���[�V�����B
    ///
    /// ParticleSystem�ƕ��s���ĊJ�n����A
    /// �A�j���[�V�����������ParticleSystem���I������܂őҋ@����B
    /// </summary>
    [SerializeField]
    private PlayerDisappearAnimation _playerDisappearAnimation;

    /*
     * Player���p���I�ɉ�]������A�j���[�V�����B
     *
     * ���݂͎g�p����Ă��Ȃ����߃R�����g�A�E�g����Ă���B
     */
    //[SerializeField]
    //private ContinuousRotateAnimation _continuousRotateAnimation;


    /* =========================================================
     * �����Q��
     * ========================================================= */

    /// <summary>
    /// Player�̃I�[����`�悵�Ă���MeshRenderer�B
    ///
    /// Awake���Ɏq�I�u�W�F�N�g����擾���A
    /// SetAuraColor�Ń}�e���A����؂�ւ���B
    /// </summary>
    private MeshRenderer _auraRenderer;


    /* =========================================================
     * ���J�v���p�e�B
     * ========================================================= */

    /// <summary>
    /// Player�����ݏ������Ă���Spline���擾����B
    /// </summary>
    public ClosedSplineLine Spline => _spline;

    /// <summary>
    /// Player�����[�J��XY���ʂ��g�p����ݒ肩�ǂ������擾����B
    /// </summary>
    public bool UseLocalPlaneXY => _useLocalPlaneXY;


    /* =========================================================
     * Unity���C�t�T�C�N��
     * ========================================================= */

    /// <summary>
    /// GameObject�������Ɉ�x�����Ăяo�����B
    ///
    /// Player�̎q�I�u�W�F�N�g����A
    /// �I�[���\���Ɏg�p����MeshRenderer���擾����B
    /// </summary>
    private void Awake()
    {
        /*
         * �q�K�w���ōŏ��Ɍ�������MeshRenderer���擾����B
         *
         * Player�̎q�ɕ�����MeshRenderer������ꍇ�A
         * �Ӑ}����Aura��Renderer�ł͂Ȃ����̂��擾�����\��������B
         *
         * �m����Aura�����𑀍삵�����ꍇ�́A
         * Inspector���璼��SerializeField�ŎQ�Ƃ�����@������B
         */
        _auraRenderer =
            GetComponentInChildren<MeshRenderer>();
    }


    /* =========================================================
     * Player���W�̍X�V
     * ========================================================= */

    /// <summary>
    /// Player�̃��[���h���W��ݒ肷��B
    ///
    /// �`�揇��Z�t�@�C�e�B���O������邽�߁A
    /// �n���ꂽ���W���Z������0.01������O�ɂ��炵�Ĕz�u����B
    /// </summary>
    /// <param name="worldPos">
    /// Player��z�u�������[���h���W�B
    /// </param>
    public void SetPosition(Vector3 worldPos)
    {
        /*
         * Spline�⑼�̕`��I�u�W�F�N�g�Ɗ��S�ɓ���Z���W�ɂȂ�ƁA
         * �`�揇���s����ɂȂ�\��������B
         *
         * ���̂��߁AZ���W��0.01��������������
         * Player���킸���Ɏ�O�֔z�u���Ă���B
         */
        worldPos = new Vector3(
            worldPos.x,
            worldPos.y,
            worldPos.z - 0.01f
        );

        // �v�Z��̃��[���h���W��Transform�֔��f����B
        transform.position = worldPos;
    }


    /* =========================================================
     * ����Spline�̍X�V
     * ========================================================= */

    /// <summary>
    /// Player�����ݏ������Ă���Spline���X�V����B
    ///
    /// �ʂ�Spline�֒��n�����ۂɁA
    /// PlayerSplineMover����Ăяo�����B
    /// </summary>
    /// <param name="spline">
    /// �V������������Spline�B
    /// </param>
    public void SetSpline(ClosedSplineLine spline)
    {
        _spline = spline;
    }


    /* =========================================================
     * �I�[���\��
     * ========================================================= */

    /// <summary>
    /// ���݂̃`���[�W���x���ɉ����āA
    /// Player�I�[���̃}�e���A����ύX����B
    ///
    /// Normal  �F��
    /// Charge1 �F�I�����W
    /// Charge2 �F��
    /// </summary>
    /// <param name="chargeLevel">
    /// ���݂�Player�̃`���[�W���x���B
    /// </param>
    internal void SetAuraColor(ChargeLevel chargeLevel)
    {
        /*
         * �z��O��ChargeLevel���n���ꂽ�ꍇ�ł�
         * null�ɂȂ�Ȃ��悤�A�����l�͒ʏ��Ԃ̐ɂ���B
         */
        Material material = _auraMaterialBlue;

        // �`���[�W���x���ɑΉ�����}�e���A����I������B
        switch (chargeLevel)
        {
            case ChargeLevel.Normal:

                material = _auraMaterialBlue;
                break;

            case ChargeLevel.Charge1:

                material = _auraMaterialOrange;
                break;

            case ChargeLevel.Charge2:

                material = _auraMaterialRed;
                break;
        }

        /*
         * Renderer������Ɏ擾�ł��Ă���ꍇ�̂݁A
         * �}�e���A����ύX����B
         *
         * sharedMaterial���g�p���Ă��邽�߁A
         * Renderer��p��Material�C���X�^���X�͐�������Ȃ��B
         *
         * ����Material���g�p���鑼Renderer�ɂ�
         * Material���̂̕ύX�͋��L����邪�A
         * ���̃R�[�h�ł͎Q�Ɛ��؂�ւ��Ă��邾���Ȃ̂Ŗ��͋N���ɂ����B
         */
        if (_auraRenderer != null)
        {
            _auraRenderer.sharedMaterial = material;
        }
    }


    /* =========================================================
     * �W�����v�����K�C�h
     * ========================================================= */

    /// <summary>
    /// Player�̃W�����v�\������������K�C�h��\������B
    ///
    /// Player�̌��݈ʒu�ƁA
    /// Spline��̊O�����@��������JumpNormalGuide�֓n���B
    /// </summary>
    /// <param name="normal">
    /// �W�����v�\������ƂȂ�@���x�N�g���B
    /// </param>
    public void ShowJumpNormalGuide(Vector3 normal)
    {
        /*
         * Player�̌��݈ʒu���K�C�h�̊J�n�n�_�Ƃ��A
         * normal�����փK�C�h��\������B
         */
        _jumpNormalGuide.Show(
            transform.position,
            normal
        );
    }

    /// <summary>
    /// �W�����v�����K�C�h���\���ɂ���B
    ///
    /// �`���[�W�I�����A�W�����v�J�n���A���S���ȂǂɌĂяo�����B
    /// </summary>
    public void HideJumpNormalGuide()
    {
        _jumpNormalGuide.Hide();
    }


    /* =========================================================
     * Spline���n�G�t�F�N�g
     * ========================================================= */

    /// <summary>
    /// Player��Spline�֒��n�����ۂ̃G�t�F�N�g���Đ�����B
    ///
    /// ���n�_�Ƀ��[�J����Burst�G�t�F�N�g���Đ����A
    /// ���K��Spline�ւ̏��񒅒n�ł����
    /// Spline�S�̂֍L����Scatter�G�t�F�N�g���Đ�����B
    /// </summary>
    /// <param name="spline">
    /// Player�����n����Spline�B
    /// </param>
    /// <param name="distance">
    /// Spline�n�_���璅�n�_�܂ł̋����B
    /// </param>
    /// <param name="hitWorldPos">
    /// �ڐG����ɂ���ċ��߂�ꂽ���n�_�̃��[���h���W�B
    ///
    /// ���݂̎����ł͎g�p����Ă��Ȃ����A
    /// BurstAtWorldPos�Ȃǂ��g�p����ꍇ�ɗ��p�ł���B
    /// </param>
    public void PlaySplineAttachFx(
        ClosedSplineLine spline,
        float distance,
        Vector3 hitWorldPos)
    {
        // ���n��Spline�����݂��Ȃ���Ώ����ł��Ȃ��B
        if (spline == null)
            return;

        /*
         * ���n����Spline����A
         * ���n�G�t�F�N�g���Ǘ�����SplineBurstEmitter���擾����B
         */
        var emitter =
            spline.GetComponent<SplineBurstEmitter>();

        // SplineBurstEmitter���t���Ă��Ȃ���Ή������Ȃ��B
        if (emitter == null)
            return;

        /*
         * Spline�n�_����distance�i�񂾒n�_�ŁA
         * ���[�J���Ȓ��nBurst�G�t�F�N�g���Đ�����B
         */
        emitter.BurstLocalAtDistance(distance);

        /*
         * ����Spline�����K�₩�J�n�n�_�ł͂Ȃ��ꍇ�A
         * Spline�S�̂ɍL����G�t�F�N�g���Đ�����B
         *
         * IsNewOrbit������false�֕ύX����邩�ɂ���ẮA
         * ���̏������������Ȃ��\�������邽�߁A
         * AttachEvent���̌Ăяo�����ɂ͒��ӂ��K�v�B
         */
        if (spline.IsNewOrbit && !spline.IsStartSpline)
        {
            emitter.ScatterGlobal();
        }

        /*
         * �ڐG�n�_�̃��[���h���W�𒼐ڎg����
         * Burst���Đ�����ꍇ�̌��B
         *
         * ���݂͎g�p����Ă��Ȃ��B
         */
        //emitter.BurstAtWorldPos(hitWorldPos);
    }


    /* =========================================================
     * Player���S���o
     * ========================================================= */

    /// <summary>
    /// Player���S���̃G�t�F�N�g�Ə����A�j���[�V�������Đ�����B
    ///
    /// �����̗���͈ȉ��B
    ///
    /// 1. GameObject�j�����ɃL�����Z�������CancellationToken���擾
    /// 2. ParticleSystem���Đ�
    /// 3. PlayerDisappearAnimation���Đ����Ċ�����҂�
    /// 4. ParticleSystem�����S�ɏI������܂ő҂�
    /// 5. Player��GameObject���A�N�e�B�u��
    ///
    /// GameObject���r���Ŕj�����ꂽ�ꍇ�́A
    /// OperationCanceledException��ߑ����ĐÂ��ɏI������B
    /// </summary>
    public async UniTask PlayDeadEffect()
    {
        /*
         * ����MonoBehaviour���j�����ꂽ�Ƃ���
         * �����I�ɃL�����Z�������CancellationToken���擾����B
         *
         * �V�[���J�ڒ���GameObject�j�����
         * �񓯊��������c�葱���邱�Ƃ�h���B
         */
        var ct =
            this.GetCancellationTokenOnDestroy();

        try
        {
            /*
             * Unity�I�u�W�F�N�g�͔j��������
             * C#��ł͎Q�Ƃ��c���Ă��Ă�this == null�ɂȂ�ꍇ������B
             */
            if (this == null)
                return;

            // ���SParticleSystem�����ݒ�Ȃ�Đ��ł��Ȃ��B
            if (_deadEffect == null)
                return;

            // ���S�p�[�e�B�N�����Đ�����B
            _deadEffect.Play();

            /*
             * Player�{�̂̏����A�j���[�V�������ݒ肳��Ă���ꍇ�A
             * �A�j���[�V�������J�n���Ċ����܂ő҂B
             */
            if (_playerDisappearAnimation != null)
            {
                await _playerDisappearAnimation
                    .PlayAsync()

                    /*
                     * GameObject���j�����ꂽ�ꍇ��
                     * �ҋ@�������L�����Z���ł���悤�ɂ���B
                     */
                    .AttachExternalCancellation(ct);
            }

            /*
             * ParticleSystem�����S�ɏI������܂ő҂B
             *
             * IsAlive(true)��true�́A
             * �qParticleSystem���܂߂čĐ������m�F����w��B
             *
             * ParticleSystem���̂��r���Ŕj�����ꂽ�ꍇ��
             * �ҋ@���I���ł���悤�Anull�`�F�b�N���܂߂Ă���B
             */
            await UniTask.WaitUntil(
                () =>
                    _deadEffect == null
                    || !_deadEffect.IsAlive(true),
                cancellationToken: ct
            );

            // �ҋ@���ɂ���GameObject���j�����ꂽ�ꍇ�͏I������B
            if (this == null)
                return;

            /*
             * ���S���o�����ׂďI���������߁A
             * Player��GameObject�S�̂��A�N�e�B�u������B
             */
            gameObject.SetActive(false);
        }
        catch (OperationCanceledException)
        {
            /*
             * GameObject�j����V�[���J�ڂɂ����
             * CancellationToken���L�����Z�����ꂽ�ꍇ�B
             *
             * �z����̏I���Ȃ̂ŁA�G���[�Ƃ��ďo�͂�����������B
             */
        }
        catch (Exception e)
        {
            /*
             * �L�����Z���ȊO�̗\�����Ȃ���O�����������ꍇ�́A
             * Unity�R���\�[���փX�^�b�N�g���[�X�t���ŏo�͂���B
             */
            Debug.LogException(e);
        }
    }
}

