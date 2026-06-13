(function () {
    "use strict";

    const drawer = document.getElementById("drawer-practice");
    const loadButton = document.getElementById("practiceLoad");
    const submitButton = document.getElementById("practiceSubmit");
    const status = document.getElementById("practiceStatus");
    const questionHost = document.getElementById("practiceQuestions");
    const result = document.getElementById("practiceResult");
    let assessment = null;

    if (!drawer || !loadButton || !submitButton || !status || !questionHost || !result) {
        return;
    }

    function setStatus(icon, message) {
        status.innerHTML = "";
        const iconElement = document.createElement("i");
        iconElement.className = `fa ${icon}`;
        status.append(iconElement, document.createTextNode(` ${message}`));
    }

    function renderAssessment(data) {
        questionHost.innerHTML = "";
        data.questions.forEach((question, questionIndex) => {
            const card = document.createElement("section");
            card.className = "practice-question";

            const heading = document.createElement("h5");
            heading.textContent = `${questionIndex + 1}. ${question.prompt}`;
            card.appendChild(heading);

            const options = document.createElement("div");
            options.className = "practice-options";
            question.options.forEach((option, optionIndex) => {
                const label = document.createElement("label");
                label.className = "practice-option";

                const input = document.createElement("input");
                input.type = "radio";
                input.name = `practice-question-${questionIndex}`;
                input.value = optionIndex.toString();

                label.append(input, document.createTextNode(option));
                options.appendChild(label);
            });

            card.appendChild(options);
            questionHost.appendChild(card);
        });
    }

    loadButton.addEventListener("click", async function () {
        const subject = document.getElementById("practiceSubject").value;
        const difficulty = document.getElementById("practiceDifficulty").value;
        const count = document.getElementById("practiceCount").value;
        const url = new URL(drawer.dataset.questionsUrl, window.location.origin);
        url.searchParams.set("subject", subject);
        url.searchParams.set("difficulty", difficulty);
        url.searchParams.set("count", count);

        loadButton.disabled = true;
        submitButton.disabled = true;
        result.classList.remove("active");
        questionHost.innerHTML = "";
        setStatus("fa-spinner fa-spin", "Generating your assessment...");

        try {
            const response = await fetch(url, { headers: { Accept: "application/json" } });
            if (!response.ok) {
                throw new Error("The assessment could not be generated.");
            }

            assessment = await response.json();
            renderAssessment(assessment);
            submitButton.disabled = false;
            setStatus(
                "fa-circle-check",
                `${assessment.questions.length} ${assessment.subject} questions loaded from ${assessment.provider}.`);
        } catch (error) {
            assessment = null;
            setStatus("fa-triangle-exclamation", error.message || "Please try again.");
        } finally {
            loadButton.disabled = false;
        }
    });

    submitButton.addEventListener("click", function () {
        if (!assessment) {
            return;
        }

        const selectedAnswers = assessment.questions.map((_, index) =>
            questionHost.querySelector(`input[name="practice-question-${index}"]:checked`));
        const unanswered = selectedAnswers.filter(answer => !answer).length;
        if (unanswered > 0) {
            setStatus("fa-triangle-exclamation", `Answer all questions first. ${unanswered} remaining.`);
            return;
        }

        let score = 0;
        assessment.questions.forEach((question, questionIndex) => {
            const selectedIndex = Number(selectedAnswers[questionIndex].value);
            if (selectedIndex === question.correctOptionIndex) {
                score++;
            }

            const labels = questionHost
                .querySelectorAll(`input[name="practice-question-${questionIndex}"]`);
            labels.forEach(input => {
                input.disabled = true;
                const optionIndex = Number(input.value);
                if (optionIndex === question.correctOptionIndex) {
                    input.closest(".practice-option").classList.add("correct");
                } else if (input.checked) {
                    input.closest(".practice-option").classList.add("incorrect");
                }
            });
        });

        const percentage = Math.round((score / assessment.questions.length) * 100);
        result.innerHTML = "";
        const title = document.createElement("strong");
        title.textContent = `Score: ${score}/${assessment.questions.length} (${percentage}%)`;
        const message = document.createElement("p");
        message.style.margin = ".45rem 0 0";
        message.style.fontSize = ".65rem";
        message.textContent = percentage >= 70
            ? "Great work. Generate another assessment when you are ready for a new challenge."
            : "Review the highlighted correct answers, then generate another assessment and try again.";
        result.append(title, message);
        result.classList.add("active");
        submitButton.disabled = true;
        setStatus("fa-chart-simple", "Assessment marked. Review your answers below.");
        result.scrollIntoView({ behavior: "smooth", block: "center" });
    });
}());
