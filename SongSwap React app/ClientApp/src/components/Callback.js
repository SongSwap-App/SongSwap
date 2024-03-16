import { useSearchParams, useNavigate } from "react-router-dom";
import { useUser } from './UserContext';
import { useEffect } from "react";

const Callback = () => {
    const [searchParams] = useSearchParams();
    const token = searchParams.get("token")
    const { setWebToken } = useUser();
    const navigate = useNavigate()

    useEffect(() => {
        if (token) {
            setWebToken(token);
            navigate("/playlist");
        }
        else {
            navigate("/");
        }
    });
};

export default Callback;