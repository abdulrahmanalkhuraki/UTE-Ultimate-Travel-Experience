import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Compass } from 'lucide-react';
import { login } from '../services/authApi';
import { saveSession } from '../utils/auth';
import loginImage from '../assets/images/login.png';

export default function Login({ onLoginSuccess }) {
  const { t } = useTranslation();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');

    // بنقرأ القيم مباشرة من عناصر الفورم وقت الإرسال (مش من الـ state بس) لأنه بعض
    // متصفحات/إضافات الـ autofill بتعبي الحقل بصرياً بدون ما تطلق onChange، فتضل قيمة
    // الـ state فاضية حتى لو الحقل ظاهر فيه نص.
    const formData = new FormData(e.currentTarget);
    const emailValue = (formData.get('email') || email).toString().trim();
    const passwordValue = (formData.get('password') || password).toString();

    setIsSubmitting(true);
    try {
      const data = await login(emailValue, passwordValue);
      if (data.role !== 'Admin') {
        setError(t('login.notAdmin'));
        return;
      }
      saveSession(data);
      onLoginSuccess?.(data);
    } catch (err) {
      setError(err.message || t('login.failed'));
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-[var(--color-app-bg)] p-4 font-sans">
      <div className="bg-[var(--color-surface)] border border-[var(--color-border)] rounded-3xl p-8 md:p-12 w-full max-w-4xl flex flex-col md:flex-row items-center gap-12 shadow-2xl">
        <div className="w-full md:w-1/2 space-y-6">
          <div className="flex items-center gap-3">
            <div className="w-11 h-11 rounded-xl bg-[var(--color-accent-soft)] flex items-center justify-center shrink-0">
              <Compass className="w-6 h-6 text-[var(--color-accent)]" />
            </div>
            <div>
              <h1 className="text-2xl font-bold text-[var(--color-text)]">{t('login.title')}</h1>
              <p className="text-sm text-[var(--color-text-muted)]">{t('login.welcome')}</p>
            </div>
          </div>

          <form className="space-y-4" onSubmit={handleSubmit}>
            {error && (
              <div className="bg-[var(--color-danger-soft)] border border-[var(--color-danger)]/30 text-[var(--color-danger)] text-sm rounded-xl p-3">
                {error}
              </div>
            )}
            <input
              type="email"
              name="email"
              autoComplete="username"
              placeholder={t('login.email')}
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
              className="w-full bg-[var(--color-app-bg)] border border-[var(--color-border)] text-[var(--color-text)] p-4 rounded-xl focus:outline-none focus:border-[var(--color-accent)] transition"
            />
            <input
              type="password"
              name="password"
              autoComplete="current-password"
              placeholder={t('login.password')}
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              className="w-full bg-[var(--color-app-bg)] border border-[var(--color-border)] text-[var(--color-text)] p-4 rounded-xl focus:outline-none focus:border-[var(--color-accent)] transition"
            />
            <div className="text-end">
              <a href="#" className="text-[var(--color-accent-2)] text-sm hover:underline">{t('login.forgotPassword')}</a>
            </div>
            <button
              type="submit"
              disabled={isSubmitting}
              className="w-full bg-[var(--color-accent-2)] hover:opacity-90 disabled:opacity-60 disabled:cursor-not-allowed text-[var(--color-on-accent-2)] font-bold p-4 rounded-xl transition flex items-center justify-center gap-2"
            >
              {isSubmitting ? t('login.signingIn') : (<>{t('login.signIn')} <span>→</span></>)}
            </button>
          </form>
        </div>

        <div className="hidden md:flex w-1/2 justify-center">
          <img
            src={loginImage}
            alt={t('login.title')}
            className="max-w-full h-auto"
          />
        </div>
      </div>
    </div>
  );
}
