mX T_p�x諠��]�	�,[쐛�+ֳ���&}���6�x0���n8u���}��Қ4`��+	w^R����#�7Վ.^������� @�-ziU��zܝ:�� 1~�J������#��1V��!	�}��S|�	cIʖ��x*�|4u��l{&��WƳ.��5#�Ͼq��L��/�}U��Qo���L������%Xٍ�{��R3�dL2��{�r�� ���d��(�� F򟱟ɡ[�9�4��I����j�D�G/�����^��J�M/��S|>��z�~f[.�`���Ɋ��1"��<��a����q�^�8~��s�F��C&�{��<�uc��9�"]I�.X(dR�i��-�����qڙ�̍:�Y�w#����L3d&+Q�E�E��`�i�'���Q7�s�l���d�t���曶�6/.��0�o����{&�zE�X>�DJ6����͋�.T;]��>�7�6%��Θ��B�c�-��M�'ZO���J�&A��K�"���i%�.�����
{�P?�/ �8nꌃ�²<?W�&M��ex��gM �DM�[���f�P�rs���n��)���1>+��\S��1�lX�\����W]�|N�ds�Vt���f%�	�RZ0�wҐ�"�oi�7q����e޴=;s���x'l�3Z�	$Yߥ�뤹��Toj՝�Ɓd���dk� =6�F���fߍ���`5e�x����ϐ�)(i!|j[�>'�V��n�'�	ާsEa	mV��m�E��(NH&A;����1�}���ӹ7챏�gB+���Њ�CV*�4��D��܎�i���x���Oj��ǵ������~��7~K�]��b�0�)�6��B[i��ve�=�Ӹay��֘�A�o���[k�g��[��"�{�^^!�Gr��bP�&�$�7�1Е�d����?��튶Bv��}�Li+�?6{�;��K��cN������0��yIbX��R߅��k�3O�lj��r?zB�:V��ԃ�)�Ƙ{7�ve�Pk[����O(��R['��O��~Fd`�����,Xҥ�KӦ�KyZ],�,)}b,4��`�p<%��?ꧯ�VQ�RO�!5�r�_����ob.�,V��8��V�-πFX`��\�*�(b-Ɛ'7�� �`��cc����t%a��j�Z�K�(E����,��u�r��oݳ��,<��wc����yv
���cp[v�U]�(��4z(Ka2�8��з��U�Tƞ�R��n��5څ�{��Mǅ���I�9�
�Y�j9�1�n�>�?�d�2W[s�+�:�m��BM�Δ`����$�B�۷��t��p�>����*C%��
�+�j�=�YF��䜱��-N��n���&�MR�8U, ��)��GȦ2m����>��/Q]`��3Tމ�T�t"�b;��v�8����-z0Ú��!6n��hNH�۱D�^�I� ���&�=��\WkL2'bG�*� S�I�OkM�D9���gF�'������T{���㸹�➟��������}�erviceHost for the CalculatorService type and 
            // provide the base address.
            serviceHost = new ServiceHost(typeof(CalculatorService));

            // Open the ServiceHostBase to create listeners and start 
            // listening for messages.
            serviceHost.Open();
        }

        protected override void OnStop()
        {
            if (serviceHost != null)
            {
                serviceHost.Close();
                serviceHost = null;
            }
        }
    }
}
