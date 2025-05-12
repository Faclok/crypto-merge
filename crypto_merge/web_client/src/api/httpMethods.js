export async function HttpNoData(url, method, jwt) {

    try {
        var response = await fetch(url, {
            method: method, // *GET, POST, PUT, DELETE, etc.
            headers: {
                "Content-Type": "application/json",
                "Authorization": 'Bearer ' + jwt
            }
        });

        if (response.ok) {

            try {
                var responsejson = await response.json();
                return { statusSuccessful: true, isDataEmpty: false, data: responsejson, code: response.status };
            } catch (error) {
                return { statusSuccessful: true, isDataEmpty: true, error: error, code: response.status };
            }
        }
        else return { statusSuccessful: false, error: response.status, code: response.status };

    } catch (error) {
        return { statusSuccessful: false, error: error, code: response.status };
    }
}

export async function HttpData(url, method, jwt, body) {

    try {
        var response = await fetch(url, {
            method: method,
            headers: {
                "Content-Type": "application/json",
                "Authorization": 'Bearer ' + jwt
            },
            body: JSON.stringify(body),
        });

        if (response.ok) {

            try {
                var responsejson = await response.json();
                return { statusSuccessful: true, isDataEmpty: false, data: responsejson, code: response.status };
            } catch (error) {
                return { statusSuccessful: true, isDataEmpty: true, error: error, code: response.status };
            }
        }
        else return { statusSuccessful: false, error: response.status, code: response.status };

    } catch (error) {
        return { statusSuccessful: false, error: error, code: response.status };
    }
}

export async function DownloadFile(url, method, jwt, body, fileName) {

    try {
        var response = await fetch(url, {
            method: method,
            headers: {
                "Content-Type": "application/json",
                "Authorization": 'Bearer ' + jwt
            },
            body: JSON.stringify(body),
        });

        if (!response.ok)
            return { statusSuccessful: false, error: response.status };

            try {

                const blob = await response.blob();
                const urlTemp = window.URL.createObjectURL(new Blob([blob]));
                const link = document.createElement('a');
                link.href = urlTemp;
                link.setAttribute('download', fileName);
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);

                return { statusSuccessful: true, isSave: true};
            } catch (error) {
                return { statusSuccessful: true, isSave: false };
            }
    } catch (error) {
        return { statusSuccessful: false, error: error };
    }
}

export function getRandomColor() {
    // Гарантируем, что цвет не слишком темный или светлый
    var getComponent = function () {
        var value = Math.floor(Math.random() * 256);
        return (value < 16 ? '0' : '') + value.toString(16);
    };

    var color = '#' + getComponent() + getComponent() + getComponent();
    return color;
}

export function getRandomInt(min, max) {
    const minCeiled = Math.ceil(min);
    const maxFloored = Math.floor(max);
    return Math.floor(Math.random() * (maxFloored - minCeiled) + minCeiled); // The maximum is exclusive and the minimum is inclusive
  }