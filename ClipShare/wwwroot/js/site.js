
// GLobal JavaScript funcitons

function formatView(views) {
    if (views >= 1000000) {
        return Math.floor(views / 1000000) + " M views";
    } else if (views >= 1000) {
        return Math.floor(views / 1000) + " K views";
    } else {
        return views + (views > 1 ? " views" : " view");
    }
}

function timeAgo(dateString, utcDateTimeNowString) {
    const dateTime = new Date(dateString); // Parse the date string
    const now = new Date(utcDateTimeNowString);
    const timeSpan = now - dateTime;
    const totalSeconds = Math.floor(timeSpan / 1000);
    const minutes = Math.floor(totalSeconds / 60);
    const hours = Math.floor(minutes / 60);
    const days = Math.floor(hours / 24);
    const months = Math.floor(days / 30);
    const years = Math.floor(days / 365);

    if (totalSeconds < 60) {
        return "Just now";
    } else if (minutes < 60) {
        return `${minutes} minute${minutes !== 1 ? 's' : ''} ago`;
    } else if (hours < 24) {
        return `${hours} hour${hours !== 1 ? 's' : ''} ago`;
    } else if (days < 30) {
        return `${days} day${days !== 1 ? 's' : ''} ago`;
    } else if (days < 365) {
        return `${months} month${months !== 1 ? 's' : ''} ago`;
    } else {
        return `${years} year${years !== 1 ? 's' : ''} ago`;
    }
}