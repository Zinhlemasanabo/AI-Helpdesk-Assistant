const problemInput = document.getElementById("problemInput");
const analyseButton = document.getElementById("analyseButton");

const categoryResult = document.getElementById("categoryResult");
const priorityResult = document.getElementById("priorityResult");
const solutionResult = document.getElementById("solutionResult");


// ========================================
// ANALYSE PROBLEM
// ========================================

analyseButton.addEventListener("click", async () => {

    const problem = problemInput.value.trim();

    if (problem === "") {
        alert("Please describe your IT problem first.");
        problemInput.focus();
        return;
    }

    // Loading state
    analyseButton.disabled = true;
    analyseButton.textContent = "Analysing...";

    categoryResult.textContent = "Analysing...";
    priorityResult.textContent = "Analysing...";
    solutionResult.textContent =
        "ResolveAI is analysing your IT problem. Please wait...";

    try {

        const response = await fetch("/api/Helpdesk/analyse", {
            method: "POST",

            headers: {
                "Content-Type": "application/json"
            },

            body: JSON.stringify({
                problem: problem
            })
        });


        if (!response.ok) {
            throw new Error("Unable to analyse the problem.");
        }


        const data = await response.json();


        categoryResult.textContent =
            data.category || "Unknown";

        priorityResult.textContent =
            data.priority || "Unknown";

        solutionResult.textContent =
            data.suggestedSolution ||
            "No solution was returned.";

    }
    catch (error) {

        categoryResult.textContent = "Error";
        priorityResult.textContent = "—";

        solutionResult.textContent =
            "ResolveAI could not analyse your problem. Please try again.";

        console.error(error);
    }
    finally {

        analyseButton.disabled = false;
        analyseButton.textContent = "✦ Analyse Problem";
    }
});


// ========================================
// EXAMPLE PROBLEMS
// ========================================

const exampleButtons =
    document.querySelectorAll(".examples button");

exampleButtons.forEach(button => {

    button.addEventListener("click", () => {

        problemInput.value = button.textContent.trim();

        problemInput.focus();
    });

});