// //import { useState }from 'react';

// export default function Login() {
//   return (
//     <div className="min-h-screen flex items-center justify-center bg-[#0f1117] p-4">
//       {/* الكارت الرئيسي */}
//       <div className="bg-[#1a1c24] border border-[#2d303e] rounded-3xl p-8 md:p-12 w-full max-w-4xl flex flex-col md:flex-row items-center gap-12 shadow-2xl">
        
//         {/* جهة اليسار: النموذج */}
//         <div className="w-full md:w-1/2 space-y-6">
//           <div className="space-y-2">
//             <h1 className="text-3xl font-bold text-white">تسجيل الدخول</h1>
//             <p className="text-slate-400">مرحباً بك مجدداً في لوحة تحكم UTE Tourism</p>
//           </div>

//           <form className="space-y-4">
//             <input 
//               type="email" 
//               placeholder="البريد الالكتروني" 
//               className="w-full bg-[#0f1117] border border-[#2d303e] text-white p-4 rounded-xl focus:outline-none focus:border-[#91B3FA] transition"
//             />
//             <input 
//               type="password" 
//               placeholder="كلمة المرور" 
//               className="w-full bg-[#0f1117] border border-[#2d303e] text-white p-4 rounded-xl focus:outline-none focus:border-[#91B3FA] transition"
//             />
//             <div className="text-right">
//               <a href="#" className="text-[#F4A261] text-sm hover:underline">نسيت كلمة المرور؟</a>
//             </div>
//             <button className="w-full bg-[#F4A261] hover:bg-[#e09258] text-white font-bold p-4 rounded-xl transition flex items-center justify-center gap-2">
//               تسجيل الدخول <span>→</span>
//             </button>
//           </form>

//           {/* تسجيل الدخول البديل */}
//           <div className="space-y-4">
//             <p className="text-center text-slate-500 text-sm">او سجل الدخول عبر</p>
//             <div className="flex justify-center gap-4">
//               {['G', 'f', ''].map((icon, i) => (
//                 <div key={i} className="w-12 h-12 flex items-center justify-center rounded-full bg-[#0f1117] border border-[#2d303e] text-white cursor-pointer hover:border-[#91B3FA] transition">
//                   {icon}
//                 </div>
//               ))}
//             </div>
//           </div>
//         </div>

//         {/* جهة اليمين: الرسم التوضيحي (الآدمن واللابتوب) */}
//         <div className="hidden md:flex w-1/2 justify-center">
//            {/* هنا يمكنك وضع الصورة التي اتفقنا عليها */}
//            <div className="w-64 h-64 bg-[#2d303e] rounded-full flex items-center justify-center border-4 border-[#91B3FA]/20">
//              <span className="text-white text-6xl">💻</span>
//            </div>
//         </div>
//       </div>
//     </div>
//   );
// }


import { useState } from 'react';
import { login } from './services/authApi';
import { saveSession } from './utils/auth';

export default function Login({ onLoginSuccess }) {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setIsSubmitting(true);
    try {
      const data = await login(email, password);
      saveSession(data);
      onLoginSuccess?.(data);
    } catch (err) {
      setError(err.message || 'Login failed. Please check your credentials.');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-[#0f1117] p-4 font-sans">
      {/* Main Card */}
      <div className="bg-[#1a1c24] border border-[#2d303e] rounded-3xl p-8 md:p-12 w-full max-w-4xl flex flex-col md:flex-row items-center gap-12 shadow-2xl">
        
        {/* Left Side: Form */}
        <div className="w-full md:w-1/2 space-y-6">
          <div className="space-y-2">
            <h1 className="text-3xl font-bold text-white">Login</h1>
            <p className="text-slate-400">Welcome back to UTE Tourism Admin Dashboard</p>
          </div>

          <form className="space-y-4" onSubmit={handleSubmit}>
            {error && (
              <div className="bg-red-500/10 border border-red-500/30 text-red-400 text-sm rounded-xl p-3">
                {error}
              </div>
            )}
            <input
              type="email"
              placeholder="Email Address"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
              className="w-full bg-[#0f1117] border border-[#2d303e] text-white p-4 rounded-xl focus:outline-none focus:border-[#91B3FA] transition"
            />
            <input
              type="password"
              placeholder="Password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              className="w-full bg-[#0f1117] border border-[#2d303e] text-white p-4 rounded-xl focus:outline-none focus:border-[#91B3FA] transition"
            />
            <div className="text-right">
              <a href="#" className="text-[#F4A261] text-sm hover:underline">Forgot password?</a>
            </div>
            <button
              type="submit"
              disabled={isSubmitting}
              className="w-full bg-[#F4A261] hover:bg-[#e09258] disabled:opacity-60 disabled:cursor-not-allowed text-white font-bold p-4 rounded-xl transition flex items-center justify-center gap-2"
            >
              {isSubmitting ? 'Signing In...' : (<>Sign In <span>→</span></>)}
            </button>
          </form>

          {/* Social Login */}
          <div className="space-y-4">
            <p className="text-center text-slate-500 text-sm">Or sign in with</p>
            <div className="flex justify-center gap-4">
              {['Google', 'Facebook', 'Apple'].map((platform, i) => (
                <div key={i} className="px-6 py-3 flex items-center justify-center rounded-xl bg-[#0f1117] border border-[#2d303e] text-white cursor-pointer hover:border-[#91B3FA] transition text-sm">
                  {platform}
                </div>
              ))}
            </div>
          </div>
        </div>

        {/* Right Side: Visual Illustration */}

    <div style={{ textAlign: "center" }}>
      <img
        src={"src/assets/images/login.png"}
        alt="Login security with lock and key"
        style={{ maxWidth: "100%", height: "auto" }}
      />
    
        {/* <div className="hidden md:flex w-1/2 justify-center">
           <div className="w-64 h-64 bg-[#2d303e] rounded-3xl flex items-center justify-center border-4 border-[#91B3FA]/20 shadow-inner">
             <span className="text-white text-7xl">💻</span>
           </div>
        </div> */}
        </div>
      </div>
    </div>
  );
}