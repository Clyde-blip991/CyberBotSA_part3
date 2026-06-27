using System;
using System.Collections.Generic;

namespace CyberBotSA_part2
{
    public class QuizQuestion
    {
        public string Question { get; set; }
        public List<string> Options { get; set; }
        public string CorrectAnswer { get; set; }
        public string Explanation { get; set; }
        public bool IsTrueFalse { get; set; }
    }

    public class QuizManager
    {
        private List<QuizQuestion> questions = new List<QuizQuestion>();
        private int currentQuestionIndex = 0;
        private int score = 0;
        public bool IsQuizActive { get; private set; } = false;

        public QuizManager()
        {
            LoadQuestions();
        }

        private void LoadQuestions()
        {
            questions.Add(new QuizQuestion
            {
                Question = "What should you do if you receive an email asking for your password?",
                Options = new List<string> { "A) Reply with your password", "B) Delete the email", "C) Report the email as phishing", "D) Ignore it" },
                CorrectAnswer = "C",
                Explanation = "Reporting phishing emails helps prevent scams and protects others.",
                IsTrueFalse = false
            });
            questions.Add(new QuizQuestion
            {
                Question = "True or False: Using the same password for multiple accounts is safe.",
                Options = new List<string> { "A) True", "B) False" },
                CorrectAnswer = "B",
                Explanation = "Using the same password means if one account is hacked, all accounts are at risk.",
                IsTrueFalse = true
            });
            questions.Add(new QuizQuestion
            {
                Question = "What does HTTPS in a website URL indicate?",
                Options = new List<string> { "A) The website is fast", "B) The website is secure", "C) The website is free", "D) The website is popular" },
                CorrectAnswer = "B",
                Explanation = "HTTPS means the website uses encryption to protect your data.",
                IsTrueFalse = false
            });
            questions.Add(new QuizQuestion
            {
                Question = "True or False: Public Wi-Fi is always safe to use for banking.",
                Options = new List<string> { "A) True", "B) False" },
                CorrectAnswer = "B",
                Explanation = "Public Wi-Fi can be intercepted by attackers. Always use a VPN.",
                IsTrueFalse = true
            });
            questions.Add(new QuizQuestion
            {
                Question = "What is two-factor authentication (2FA)?",
                Options = new List<string> { "A) Two passwords", "B) A backup email", "C) An extra security step beyond a password", "D) A type of antivirus" },
                CorrectAnswer = "C",
                Explanation = "2FA adds an extra layer of security by requiring a second verification step.",
                IsTrueFalse = false
            });
            questions.Add(new QuizQuestion
            {
                Question = "True or False: Antivirus software can protect against all cyber threats.",
                Options = new List<string> { "A) True", "B) False" },
                CorrectAnswer = "B",
                Explanation = "Antivirus helps but cannot protect against all threats. User awareness is also essential.",
                IsTrueFalse = true
            });
            questions.Add(new QuizQuestion
            {
                Question = "What is social engineering in cybersecurity?",
                Options = new List<string> { "A) Building social media apps", "B) Manipulating people to reveal confidential info", "C) Engineering social networks", "D) Creating user profiles" },
                CorrectAnswer = "B",
                Explanation = "Social engineering tricks people into giving away sensitive information.",
                IsTrueFalse = false
            });
            questions.Add(new QuizQuestion
            {
                Question = "True or False: You should click links in emails from unknown senders.",
                Options = new List<string> { "A) True", "B) False" },
                CorrectAnswer = "B",
                Explanation = "Links in emails from unknown senders could lead to phishing sites or malware.",
                IsTrueFalse = true
            });
            questions.Add(new QuizQuestion
            {
                Question = "Which of these is the strongest password?",
                Options = new List<string> { "A) password123", "B) John1990", "C) P@ssw0rd!", "D) Tr0ub4dor&3" },
                CorrectAnswer = "D",
                Explanation = "A long passphrase with mixed characters is the strongest type of password.",
                IsTrueFalse = false
            });
            questions.Add(new QuizQuestion
            {
                Question = "What is ransomware?",
                Options = new List<string> { "A) Free software", "B) Malware that encrypts files and demands payment", "C) A type of antivirus", "D) A secure browser" },
                CorrectAnswer = "B",
                Explanation = "Ransomware locks your files and demands payment. Always back up your data.",
                IsTrueFalse = false
            });
            questions.Add(new QuizQuestion
            {
                Question = "True or False: POPIA protects South African citizens personal data.",
                Options = new List<string> { "A) True", "B) False" },
                CorrectAnswer = "A",
                Explanation = "The Protection of Personal Information Act (POPIA) protects SA citizens data rights.",
                IsTrueFalse = true
            });
        }

        public string StartQuiz()
        {
            IsQuizActive = true;
            currentQuestionIndex = 0;
            score = 0;
            return "Quiz started! Let's test your cybersecurity knowledge.\n\n" + GetCurrentQuestion();
        }

        public string GetCurrentQuestion()
        {
            if (currentQuestionIndex >= questions.Count)
                return EndQuiz();

            QuizQuestion q = questions[currentQuestionIndex];
            string result = $"Question {currentQuestionIndex + 1} of {questions.Count}:\n\n";
            result += q.Question + "\n\n";
            foreach (string option in q.Options)
                result += option + "\n";
            return result;
        }

        public string AnswerQuestion(string answer)
        {
            if (!IsQuizActive)
                return "No quiz is active. Type 'start quiz' to begin.";

            if (currentQuestionIndex >= questions.Count)
                return EndQuiz();

            QuizQuestion q = questions[currentQuestionIndex];
            answer = answer.Trim().ToUpper();

            string result = "";
            if (answer == q.CorrectAnswer)
            {
                score++;
                result = "Correct! " + q.Explanation + "\n\n";
            }
            else
            {
                result = $"Incorrect. The correct answer was {q.CorrectAnswer}. {q.Explanation}\n\n";
            }

            currentQuestionIndex++;

            if (currentQuestionIndex >= questions.Count)
                result += EndQuiz();
            else
                result += GetCurrentQuestion();

            return result;
        }

        private string EndQuiz()
        {
            IsQuizActive = false;
            string feedback = "";
            if (score >= 9)
                feedback = "Outstanding! You're a cybersecurity pro!";
            else if (score >= 7)
                feedback = "Great job! You have solid cybersecurity knowledge!";
            else if (score >= 5)
                feedback = "Good effort! Keep learning to stay safe online.";
            else
                feedback = "Keep learning! Cybersecurity awareness is important.";

            return $"Quiz complete! Your score: {score}/{questions.Count}\n\n{feedback}";
        }
    }
}
