async function fetchNewPosters() {
    const container = document.getElementById('movieContainer');
    container.innerHTML = "";
    try {
        const response = await fetch('/api/movie/new-posters');
        if (response.ok) {
            const posters = await response.json();

            posters.forEach(poster => {
                const col = document.createElement('div');
                col.className = 'col-md-3 mb-4';

                col.innerHTML = `
                    <div class="card shadow-sm">
                        <img src="${poster}" class="card-img-top" alt="Movie Poster">
                        <div class="card-body text-center">
                            <h5 class="card-title">New Movie</h5>
                        </div>
                    </div>
                `;

                container.appendChild(col);
            });
        }
    } catch (error) {
        console.error('Error fetching new posters:', error);
    }
}

// Fetch posters every 2 seconds
setInterval(fetchNewPosters, 2000);

document.addEventListener("DOMContentLoaded", () => {
    const addFilmButton = document.getElementById("addFilmButton");
    const searchInput = document.getElementById("searchInput");

    addFilmButton.addEventListener("click", async () => {
        const filmName = searchInput.value.trim();

        if (!filmName) {
            alert("Please enter a film name!");
            return;
        }

        try {
            const response = await fetch('/api/movie/add-to-queue', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ filmName })
            });

            if (response.ok) {
                alert("Film added to the queue successfully!");
                searchInput.value = ""; // Clear input after adding
            } else {
                alert("Failed to add film to the queue.");
            }
        } catch (error) {
            console.error("Error adding film to the queue:", error);
        }
    });
});
