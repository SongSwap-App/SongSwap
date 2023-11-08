import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

function Callback() {
    const navigate = useNavigate();

    useEffect(() => {
        const searchParams = new URLSearchParams(window.location.search);
        const data64 = searchParams.get('data64');

        if (data64) {
            const decodedData = atob(data64);

            try {
                const parsedData = JSON.parse(decodedData);

                if (parsedData.authModel.status === 'success') {
                    navigate(`/${parsedData.integrationUserUUID}`);
                } else {
                    alert('Something went wrong');
                    navigate('/');
                }
            } catch (error) {
                console.error('Error parsing JSON:', error);
            }
        }
    }, [navigate]);

    return null;
}

export default Callback;
